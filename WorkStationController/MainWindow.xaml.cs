using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WorkstationController.Control;
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
        private InstrumentsManager _instrumentsManager = InstrumentsManager.Instance;
        private List<Command> _supportedCommands = null;

        // Dynamic tab items
        private List<TabItem> _tabItems = new List<TabItem>();
        private TabItem _tabAdd = new TabItem();
        #endregion

        /// <summary>
        /// Reference the InstrumentManager single instance
        /// </summary>
        public InstrumentsManager InstrumentsManager
        {
            get
            {
                return this._instrumentsManager;
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
            try
            {
                this._instrumentsManager.Initialize();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "启动失败！");
                return;
            }
            this._supportedCommands = Command.CreatSupportedCommands();

            // Set the data context of the dialog
            this.DataContext = this;
        }

        #region Dynamic TabItem
        private TabItem AddTabItem(UserControl control)
        {
            if (control == null)
                return null;

            // create new tab item
            TabItem tab = new TabItem();
            
            // Set the Tag of the tab as the UID of the instrument
            if(control.DataContext is WareBase)
            {
                WareBase ware = control.DataContext as WareBase;
                tab.Tag = ware.ID;
            }
            else if (control.DataContext is LiquidClass)
            {
                LiquidClass liquidClass = control.DataContext as LiquidClass;
                tab.Tag = liquidClass.ID;
            }
            else if(control.DataContext == null)     // Tempeorary fix of new a recipe tab item.
            {
                tab.Tag = Guid.NewGuid();
            }
 
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            //StackPanel stackpanel = new StackPanel();
            //stackpanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //stackpanel.Children.Add(control);
            Grid grid = new Grid();
            grid.Children.Add(control);
            tab.Content = grid;

            // insert tab at most left position
            _tabItems.Insert(0, tab);

            tabDynamic.DataContext = null;
            tabDynamic.DataContext = _tabItems;
            tabDynamic.SelectedItem = tab;

            return tab;
        }

        private void DeleteTabItem(Guid tag)
        {
            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tag)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                // get selected tab
                TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                // clear tab control binding
                tabDynamic.DataContext = null;

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

        private bool ActivateEditingTab(Guid tag)
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
            Guid tabName = (Guid)(sender as Button).CommandParameter;
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
            if (this.ActivateEditingTab(selectedLW.ID))
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
            InstrumentsManager.Instance.DeleteInstrument<Labware>(selectedLW.ID);
            DeleteTabItem(selectedLW.ID);
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
            if (this.ActivateEditingTab(selectedCr.ID))
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
            InstrumentsManager.Instance.DeleteInstrument<Carrier>(selectedCr.ID);
            DeleteTabItem(selectedCr.ID);
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
            if (this.ActivateEditingTab(selectedRecipe.ID))
                return;

            RecipeEditor editor = new RecipeEditor(selectedRecipe);
            this.AddTabItem(editor);
        }

        private void OnRecipesNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            RecipeEditor editor = new RecipeEditor();
            editor.DataContext = null;
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
            if (this.ActivateEditingTab(selectedLC.ID))
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
            InstrumentsManager.Instance.DeleteInstrument<LiquidClass>(selectedLC.ID);
            DeleteTabItem(selectedLC.ID);
        }
        #endregion

        #region Carrier preview Left button down
        private void lb_carriers_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(lb_carriers);
            HitTestResult result = VisualTreeHelper.HitTest(lb_carriers, pt);
            ListBoxItem lbi = VisualCommon.FindParent<ListBoxItem>(result.VisualHit);
            if (lbi == null)
                return;
            int index = lb_carriers.ItemContainerGenerator.IndexFromContainer(lbi);
            if (index == -1)
                return;

            TabItem tabitem = (TabItem)tabDynamic.SelectedItem;
            RecipeEditor recipeEditor = GetRecipeEditor();
            if (recipeEditor == null)
                return;
            recipeEditor.SuggestCandidate((Carrier)lb_carriers.Items[index]);
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

        #region dispose
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }
    }
}