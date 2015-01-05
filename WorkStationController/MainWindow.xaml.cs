﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Control;
using WorkstationController.Core;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IDisposable
    {
        #region Private members
        private PipettorElementManager _pipettorElementManager = PipettorElementManager.Instance;
        private List<Command> _supportedCommands = null;

        // Dynamic tab items
        private List<TabItem> _tabItems = new List<TabItem>();
        private TabItem _tabAdd = new TabItem();
        #endregion

        #region Commands
        private static RoutedUICommand _save_pipettorElements = null;

        /// <summary>
        /// Save command
        /// </summary>
        public static RoutedUICommand SavePipettorElements
        {
            get { return _save_pipettorElements; }
        }

        private void SavePipettorElements_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SavePipettorElements_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._tabItems.Count > 0;
        }

        static MainWindow()
        {
            InputGestureCollection ic_save_pipettorElements = new InputGestureCollection();
            ic_save_pipettorElements.Add(new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"));
            _save_pipettorElements = new RoutedUICommand("Save", "SavePipettorElements", typeof(MainWindow), ic_save_pipettorElements);
        }
        #endregion

        /// <summary>
        /// Reference the InstrumentManager single instance
        /// </summary>
        public PipettorElementManager PipettorElementsManager
        {
            get
            {
                return this._pipettorElementManager;
            }
        }

        /// <summary>
        /// Gets the supported commands
        /// </summary>
        public List<Command> SupportedCommands
        {
            get { return this._supportedCommands; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize pipettor element manager
            try
            {
                this._pipettorElementManager.Initialize();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.StartupFailed);
                return;
            }

            // Initialize supported command
            this._supportedCommands = Command.CreatSupportedCommands();

            // Set the data context of the dialog
            this.DataContext = this;

            // Event subscribe
            this.Loaded += OnMainWindowLoaded;
            this.Closing += OnMainWindowClosing;
        }

        #region Event handler
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Commands binding
            this.CommandBindings.Add(new CommandBinding(SavePipettorElements, this.SavePipettorElements_Executed, this.SavePipettorElements_CanExecute));
        }

        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Dispose();
        }
        #endregion

        #region Dynamic TabItem
        private TabItem AddTabItem(UserControl control)
        {
            if (control == null)
                return null;

            // create new tab item
            TabItem tab = new TabItem();
            
            // Set the Tag of the tab as the UID of the instrument
            tab.Tag = ((PipettorElement)control.DataContext).SaveName;
 
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            grid.Children.Add(control);
            tab.Content = grid;

            // insert tab at most left position
            _tabItems.Insert(0, tab);

            tabDynamic.DataContext = null;
            tabDynamic.DataContext = _tabItems;
            tabDynamic.SelectedItem = tab;

            return tab;
        }

        private void DeleteTabItem(string tag)
        {
            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tag)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                // get selected tab
                TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                // clear tab control binding
                tabDynamic.DataContext = null;

                //call dispose for the editor
                object editor = ((Grid)tab.Content).Children[0];
                if (typeof(IDisposable).IsAssignableFrom(editor.GetType()))
                {
                    ((IDisposable)editor).Dispose();
                }

                _tabItems.Remove(tab);

                // bind tab control
                tabDynamic.DataContext = _tabItems;

                // select previously selected tab. if that is removed then select first tab
                if ((selectedTab == null || selectedTab.Equals(tab)) && _tabItems.Count > 0)
                {
                    selectedTab = _tabItems[0];
                }
                tabDynamic.SelectedItem = selectedTab;
            }
        }

        private bool ActivateEditingTab(string tag)
        {
            bool isEditing = false;

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tag)).SingleOrDefault();

            TabItem tabItem = item as TabItem;

            if (tabItem != null)
            {
                tabDynamic.SelectedItem = tabItem;
                isEditing = true;
            }

            return isEditing;
        }


        private void OnCommandTabItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            System.Collections.IList items = e.AddedItems;
            if(items.Count > 0 && items[0] is TabItem)
            {
                TabItem tabitem = (TabItem)items[0];
                RecipeEditor layoutEditor = ((Grid)tabitem.Content).Children[0] as RecipeEditor;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (string)(sender as Button).CommandParameter;
            DeleteTabItem(tabName);
        }
        #endregion

        #region Labware context menu
        private void OnLabwaresMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnLabwareEditMenuItemClick(sender, e);
        }

        private void OnLabwareEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            Labware selectedLW = (Labware)this.lb_labwares.SelectedItem;
            if (this.ActivateEditingTab(selectedLW.TypeName))
                return;

            LabwareEditor editor = new LabwareEditor();
            editor.DataContext = selectedLW;
            this.AddTabItem(editor);
        }

        private void OnLabwareNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            Labware labware = new Labware();
            LabwareEditor editor = new LabwareEditor();
            editor.DataContext = labware;
            this.AddTabItem(editor);
        }

        private void OnLabwareDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
            Labware labware = ((Labware)this.lb_labwares.SelectedItem).Clone() as Labware;
            LabwareEditor editor = new LabwareEditor();
            editor.DataContext = labware;
            this.AddTabItem(editor);
        }

        private void OnLabwareDeleteMenuItemClick(object sender, RoutedEventArgs e)
        {
            Labware selectedLW = (Labware)this.lb_labwares.SelectedItem;
            PipettorElementManager.Instance.DeletePipettorElement<Labware>(selectedLW.TypeName);
            DeleteTabItem(selectedLW.TypeName);
        }
        #endregion

        #region Carrier context menu
        private void OnCarriersMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnCarrierEditMenuItemClick(sender, e);
        }

        private void OnCarrierEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier selectedCr = (Carrier)this.lb_carriers.SelectedItem;
            if (this.ActivateEditingTab(selectedCr.TypeName))
                return;

            CarrierEditor editor = new CarrierEditor();
            editor.DataContext = selectedCr;
            this.AddTabItem(editor);
        }

        private void OnCarrierNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier carrier = new Carrier();
            CarrierEditor editor = new CarrierEditor();
            editor.DataContext = carrier;
            this.AddTabItem(editor);
        }

        private void OnCarrierDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier carrier = ((Carrier)this.lb_carriers.SelectedItem).Clone() as Carrier;
            CarrierEditor editor = new CarrierEditor();
            editor.DataContext = carrier;
            this.AddTabItem(editor);
        }

        private void OnCarrierDeleteMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier selectedCr = (Carrier)this.lb_carriers.SelectedItem;
            if (selectedCr == null)
                return;
            PipettorElementManager.Instance.DeletePipettorElement<Carrier>(selectedCr.TypeName);
            DeleteTabItem(selectedCr.TypeName);
        }
        #endregion

        #region Recipes context menu
        private void OnRecipesMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnRecipesEditMenuItemClick(sender, e);
        }

        private void OnRecipesEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)this.lb_recipes.SelectedItem;
            if (this.ActivateEditingTab(selectedRecipe.Name))
                return;

            RecipeEditor editor = new RecipeEditor(selectedRecipe);
            editor.DataContext = selectedRecipe;
            this.AddTabItem(editor);
        }

        private void OnRecipesNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            RecipeEditor editor = new RecipeEditor();
            editor.DataContext = editor.Recipe;
            this.AddTabItem(editor);
        }

        private void OnRecipesDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnRecipesDeleteMenuItemClick(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region LiquidClass context menu
        private void OnLiquidclassMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnLiquidClassEditMenuItemClick(sender, e);
        }

        private void OnLiquidClassEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass selectedLC = (LiquidClass)this.lb_liquidclass.SelectedItem;
            if (this.ActivateEditingTab(selectedLC.TypeName))
                return;

            LiquidClassEditor editor = new LiquidClassEditor();
            editor.DataContext = selectedLC;
            this.AddTabItem(editor);
        }

        private void OnLiquidClassNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass liquidClass = new LiquidClass();
            LiquidClassEditor editor = new LiquidClassEditor();
            editor.DataContext = liquidClass;
            this.AddTabItem(editor);
        }

        private void OnLiquidClassDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass liquidClass = ((LiquidClass)this.lb_liquidclass.SelectedItem).Clone() as LiquidClass;
            LiquidClassEditor editor = new LiquidClassEditor();
            editor.DataContext = liquidClass;
            this.AddTabItem(editor);
        }

        private void OnLiquidClassDeleteMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass selectedLC = (LiquidClass)this.lb_liquidclass.SelectedItem;
            PipettorElementManager.Instance.DeletePipettorElement<LiquidClass>(selectedLC.TypeName);
            DeleteTabItem(selectedLC.TypeName);
        }
        #endregion

        #region preview Left button down
        private void OnLeftButtonDown(ListBox listBox, Point pt)
        {
            TabItem tabitem = (TabItem)tabDynamic.SelectedItem;
            RecipeEditor recipeEditor = GetRecipeEditor();
            if (recipeEditor == null)
                return;

            HitTestResult result = VisualTreeHelper.HitTest(listBox, pt);
            ListBoxItem lbi = VisualCommon.FindParent<ListBoxItem>(result.VisualHit);
            if (lbi == null)
                return;
            int index = listBox.ItemContainerGenerator.IndexFromContainer(lbi);
            if (index == -1)
                return;

            recipeEditor.SuggestCandidate((WareBase)listBox.Items[index]);
        }

        private void lb_carriers_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool isDoubleClick = e.ClickCount > 1;
            if (isDoubleClick)
                return;
            Point pt = e.GetPosition(lb_carriers);
            OnLeftButtonDown(lb_carriers, pt);
        }

        private void lb_labwares_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool isDoubleClick = e.ClickCount > 1;
            if (isDoubleClick)
                return;
            Point pt = e.GetPosition(lb_labwares);
            OnLeftButtonDown(lb_labwares, pt);
        }
        #endregion

        private RecipeEditor GetRecipeEditor()
        {
            //TabItem tabitem = (TabItem)tabDynamic.SelectedItem;
            RecipeEditor recipeEditor = null;
            foreach (TabItem tabItem in tabDynamic.Items)
            {
                recipeEditor = ((Grid)tabItem.Content).Children[0] as RecipeEditor;
                if (recipeEditor != null)
                    break;
            }
            return recipeEditor;
        }

        #region Dispose
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            var recipeEditor = GetRecipeEditor();
            if (recipeEditor != null)
                recipeEditor.Dispose();
        }
        #endregion      
    }
}