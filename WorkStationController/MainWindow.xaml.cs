using System;
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
    public partial class MainWindow : Window
    {
        #region Private members
        private PipettorElementManager _pipettorElementManager = PipettorElementManager.Instance;
        private List<string> _supportedCommands = null;

        // Dynamic tab items
        private List<TabItem> _tabItems = new List<TabItem>();
        private TabItem _tabAdd = new TabItem();
        #endregion

        #region Commands
      

        private void SavePipettorElements_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LayoutEditor layoutEditor = GetLayoutEditor();
            if (layoutEditor == null)
                return;
            var layout = layoutEditor.Layout;
            PipettorElementManager.Instance.SavePipettorElement(layout);
        }

        

        private void SavePipettorElements_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._tabItems.Count > 0;
        }

        private void StartScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem selectedTab = tabDynamic.SelectedItem as TabItem;
            LayoutEditor layoutEditor = (LayoutEditor)((Grid)selectedTab.Content).Children[0];
            AdvancedPipettingCommand pipettingCmd1 = new AdvancedPipettingCommand("label1", new List<int>() { 1 }, new List<int>() { 1 }, new LiquidClass(), true, 100);
            AdvancedPipettingCommand pipettingCmd2 = new AdvancedPipettingCommand("label1", new List<int>() { 1 }, new List<int>() { 9 }, new LiquidClass(), false, 100);
            AdvancedPipettingCommand pipettingCmd3 = new AdvancedPipettingCommand("label1", new List<int>() { 1 }, new List<int>() { 2 }, new LiquidClass(), true, 100);
            AdvancedPipettingCommand pipettingCmd4 = new AdvancedPipettingCommand("label1", new List<int>() { 1 }, new List<int>() { 10 }, new LiquidClass(), false, 100);
            List<IPipettorCommand> pipettingCmds = new List<IPipettorCommand>(){
                pipettingCmd1,pipettingCmd2,
                pipettingCmd3,pipettingCmd4
            };

            layoutEditor.RunScript(pipettingCmds);

        }

        private void StartScript_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsScriptExecuteCommandCanExecute();
        }

        private void ResumeScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ResumeScript_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsScriptExecuteCommandCanExecute();
        }

        private void StopScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void StopScript_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsScriptExecuteCommandCanExecute();
        }

        private bool IsScriptExecuteCommandCanExecute()
        {
            if (this._tabItems.Count > 0)
            {
                // If the selected tab is a RecipeEditor
                TabItem selectedTab = tabDynamic.SelectedItem as TabItem;
                return ((Grid)selectedTab.Content).Children[0] is LayoutEditor;
            }
            else
                return false;
        }

        private void EditLabware_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Labware selectedLabware = e.Parameter != null? (Labware)e.Parameter: (Labware)this.lb_labwares.SelectedItem;
            if (selectedLabware == null || this.ActivateEditingTab(selectedLabware.TypeName))
                return;

            LabwareEditor editor = new LabwareEditor(AddInfo);
            selectedLabware.CalculatePositionInLayout();
            editor.DataContext = selectedLabware;
            this.AddTabItem(editor);
        }


        private void SetAsCurrentDiti_Executed(object sender,ExecutedRoutedEventArgs e )
        {

        }

        private void SetDitiPosition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Labware labware = e.Parameter as Labware;
            DitiEditor ditiEditor = new DitiEditor(labware);
            ditiEditor.ShowDialog();
        }
     
       
        private void EditLabware_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.HasLabwareSelected() || e.Parameter is Labware;
        }

        private void SetAsCurrentDiti_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is Labware)
            {
                Labware labware = e.Parameter as Labware;
                if (labware.IsDitiBox)
                {
                    e.CanExecute = true;
                    return;
                }
            }
            e.CanExecute = false;

        }

        private void SetDitiPosition_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is Labware)
            {
                Labware labware = e.Parameter as Labware;
                if (labware.IsDitiBox)
                {
                    e.CanExecute = true;
                    return;
                }
            }
            e.CanExecute = false;
        }

        private void NewLabware_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Labware labware = new Labware();
            LabwareEditor editor = new LabwareEditor(AddInfo);
            editor.DataContext = labware;
            this.AddTabItem(editor);
        }

        private void NewLabware_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // New command is always enabled
            e.CanExecute = true;
        }

        private void DuplicateLabware_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Labware labware = ((Labware)this.lb_labwares.SelectedItem).Clone() as Labware;
            LabwareEditor editor = new LabwareEditor(AddInfo);
            editor.DataContext = labware;
            this.AddTabItem(editor);
        }

        private void DuplicateLabware_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.HasLabwareSelected();
        }

        private void DeleteLabware_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Labware selectedLW = (Labware)this.lb_labwares.SelectedItem;
            PipettorElementManager.Instance.DeletePipettorElement<Labware>(selectedLW.TypeName);
            DeleteTabItem(selectedLW.TypeName);
        }

        private void DeleteLabware_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.HasLabwareSelected();
        }

        private bool HasLabwareSelected()
        {
            return this.lb_labwares.SelectedItem != null && this.lb_labwares.SelectedIndex != -1;
        }

        private void EditCarrier_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EditCarrier_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Carrier selectedCarrier = (Carrier)e.Parameter;
            if (selectedCarrier == null || this.ActivateEditingTab(selectedCarrier.TypeName))
                return;

            CarrierEditor editor = new CarrierEditor(AddInfo);
            editor.DataContext = selectedCarrier;
            this.AddTabItem(editor);
        }

        static MainWindow()
        {
            // Save command
            InputGestureCollection ic_save_pipettorElements = new InputGestureCollection();
            ic_save_pipettorElements.Add(new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"));
            CommandsManager.SavePipettorElements = new RoutedUICommand("Save", "SavePipettorElements", typeof(MainWindow), ic_save_pipettorElements);

            // Start/Resume/Stop Script commands
            CommandsManager.StartScript = new RoutedUICommand("Start", "StartScript", typeof(MainWindow), null);
            CommandsManager.ResumeScript = new RoutedUICommand("Resume", "ResumeScript", typeof(MainWindow), null);
            CommandsManager.StopScript = new RoutedUICommand("Stop", "StopScript", typeof(MainWindow), null);

            // Labware commands
            CommandsManager.EditLabware = new RoutedUICommand("Edit Labware", "EditLabware", typeof(MainWindow), null);
            CommandsManager.NewLabware = new RoutedUICommand("New Labware", "NewLabware", typeof(MainWindow), null);
            CommandsManager.DuplicateLabware = new RoutedUICommand("Duplicate Labware", "DuplicateLabware", typeof(MainWindow), null);
            CommandsManager.DeleteLabware = new RoutedUICommand("Delete Labware", "DeleteLabware", typeof(MainWindow), null);

            //diti commands
            CommandsManager.SetAsCurrentDiti = new RoutedUICommand("Set as current diti box","SetAsCurrentDiti",typeof(MainWindow),null);
            CommandsManager.SetDitiPosition = new RoutedUICommand("Set diti position", "SetDitiPosition", typeof(MainWindow), null);
            // Carrier commands
            CommandsManager.EditCarrier = new RoutedUICommand("Edit Carrier", "EditCarrier", typeof(MainWindow), null);
            CommandsManager.NewCarrier = new RoutedUICommand("New Carrier", "NewCarrier", typeof(MainWindow), null);
            CommandsManager.DuplicateCarrier = new RoutedUICommand("Duplicate Carrier", "DuplicateCarrier", typeof(MainWindow), null);
            CommandsManager.DeleteCarrier = new RoutedUICommand("Delete Carrier", "DeleteCarrier", typeof(MainWindow), null);
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
        public List<string> SupportedCommands
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
            //try
            {
                this._pipettorElementManager.Initialize();
            }
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message, Properties.Resources.StartupFailed);
            //    return;
            //}

            // Initialize supported command
            this._supportedCommands = Command.CreatSupportedCommands();

            // Set the data context of the dialog
            this.DataContext = this;

            // Event subscribe
            this.Loaded += OnMainWindowLoaded;
        }

        #region Event handler
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Commands binding
            this.CommandBindings.Add(new CommandBinding(CommandsManager.SavePipettorElements, this.SavePipettorElements_Executed, this.SavePipettorElements_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.StartScript, this.StartScript_Executed, this.StartScript_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.ResumeScript, this.ResumeScript_Executed, this.ResumeScript_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.StopScript, this.StopScript_Executed, this.StopScript_CanExecute));
            
            this.CommandBindings.Add(new CommandBinding(CommandsManager.EditLabware, this.EditLabware_Executed, this.EditLabware_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.NewLabware, this.NewLabware_Executed, this.NewLabware_CanExecute)); 
            this.CommandBindings.Add(new CommandBinding(CommandsManager.DuplicateLabware, this.DuplicateLabware_Executed, this.DuplicateLabware_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.DeleteLabware, this.DeleteLabware_Executed, this.DeleteLabware_CanExecute));


            this.CommandBindings.Add(new CommandBinding(CommandsManager.SetAsCurrentDiti, this.SetAsCurrentDiti_Executed, this.EditLabware_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CommandsManager.SetDitiPosition, this.SetDitiPosition_Executed, this.NewLabware_CanExecute)); 


            this.CommandBindings.Add(new CommandBinding(CommandsManager.EditCarrier, this.EditCarrier_Executed, this.EditCarrier_CanExecute));
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

            // If a recipeEditor is added, subscribe its EditWare event
            if(control is LayoutEditor)
            {
                LayoutEditor layoutEditor = control as LayoutEditor;
                layoutEditor.EditWare += OnLayoutEditorEditWare;
            }
            control.Width = this.ActualWidth - 300;
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

        private void OnLayoutEditorEditWare(object sender, WareBase e)
        {
            if(e is Labware)
            {
                Labware labware = e as Labware;
                if (this.ActivateEditingTab(labware.TypeName))
                    return;

                LabwareEditor editor = new LabwareEditor(AddInfo);
                editor.DataContext = labware;
                this.AddTabItem(editor);
            }
            else if(e is Carrier)
            {
                Carrier carrier = e as Carrier;
                if (this.ActivateEditingTab(carrier.TypeName))
                    return;

                CarrierEditor editor = new CarrierEditor(AddInfo);
                editor.DataContext = carrier;
                this.AddTabItem(editor);
            }
            else
            {
                // Bad argument
            }
        }

        private void DeleteTabItem(string tag)
        {
            var validTags = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tag));
            if (validTags.Count() > 1)
                throw new Exception("有重名设置！");
            var item = validTags.SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                // get selected tab
                TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                // clear tab control binding
                tabDynamic.DataContext = null;

                //call dispose for the editor
                UserControl editor = (UserControl)(((Grid)tab.Content).Children[0]);
                if (typeof(IDisposable).IsAssignableFrom(editor.GetType()))
                {
                    ((IDisposable)editor).Dispose();
                }

                // If the editor is a RecipeEditor, unsubscribe its EditWare event
                if(editor is LayoutEditor)
                {
                    LayoutEditor recipeEditor = editor as LayoutEditor;
                    recipeEditor.EditWare -= this.OnLayoutEditorEditWare;
                }

                // Remove the tab
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
            TabItem item = null;
            try
            {
                var validTabs = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tag));
                item = validTabs.SingleOrDefault();
            }
            catch(Exception ex)
            {
                return false;
            }

            TabItem tabItem = item as TabItem;

            if (tabItem != null)
            {
                tabDynamic.SelectedItem = tabItem;
                isEditing = true;
            }

            return isEditing;
        }
        private void AddInfo(string sInfo, bool error = false)
        {
            rtb_Output.SelectionBrush = error ? Brushes.Red : Brushes.Black;
            rtb_Output.AppendText(sInfo);
            rtb_Output.ScrollToEnd();
        }

        private void AddErrorInfo(string sInfo)
        {
            AddInfo(sInfo, true);
        }

        private void OnCommandTabItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            System.Collections.IList items = e.AddedItems;
            if(items.Count > 0 && items[0] is TabItem)
            {
                TabItem tabitem = (TabItem)items[0];
                LayoutEditor layoutEditor = ((Grid)tabitem.Content).Children[0] as LayoutEditor;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (string)(sender as Button).CommandParameter;
            DeleteTabItem(tabName);
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
            if (selectedCr == null)
                return;

            if (this.ActivateEditingTab(selectedCr.TypeName))
                return;

            CarrierEditor editor = new CarrierEditor(AddInfo);
            editor.DataContext = selectedCr;
            this.AddTabItem(editor);
        }

        private void OnCarrierNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier carrier = new Carrier();
            CarrierEditor editor = new CarrierEditor(AddInfo);
            editor.DataContext = carrier;
            this.AddTabItem(editor);
        }

        private void OnCarrierDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
            Carrier carrier = ((Carrier)this.lb_carriers.SelectedItem).Clone() as Carrier;
            CarrierEditor editor = new CarrierEditor(AddInfo);
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

        #region Layout context menu
        private void OnLayoutsMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnLayoutsEditMenuItemClick(sender, e);
        }

        private void OnLayoutsEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            Layout selectedLayout = (Layout)this.lb_layouts.SelectedItem;
            if (selectedLayout == null)
                return;

            if (this.ActivateEditingTab(selectedLayout.SaveName))
                return;

            LayoutEditor editor = new LayoutEditor(selectedLayout,AddInfo);
            editor.DataContext = selectedLayout;
            var tabItem = this.AddTabItem(editor);
        }

        private void OnLayoutsNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            LayoutEditor editor = new LayoutEditor(null, AddInfo);
            editor.DataContext = editor.Layout;
            this.AddTabItem(editor);
        }

        private void OnLayoutsDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnLayoutsDeleteMenuItemClick(object sender, RoutedEventArgs e)
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

            LiquidClassEditor editor = new LiquidClassEditor(AddInfo);
            editor.DataContext = selectedLC;
            this.AddTabItem(editor);
        }

        private void OnLiquidClassNewMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass liquidClass = new LiquidClass();
            LiquidClassEditor editor = new LiquidClassEditor(AddInfo);
            editor.DataContext = liquidClass;
            this.AddTabItem(editor);
        }

        private void OnLiquidClassDuplicateMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClass liquidClass = ((LiquidClass)this.lb_liquidclass.SelectedItem).Clone() as LiquidClass;
            LiquidClassEditor editor = new LiquidClassEditor(AddInfo);
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
            LayoutEditor recipeEditor = GetLayoutEditor();
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
            Point pt = e.GetPosition(lb_labwares);

            HitTestResult result = VisualTreeHelper.HitTest(this.lb_labwares, pt);
            ListBoxItem lbi = VisualCommon.FindParent<ListBoxItem>(result.VisualHit);
            if (lbi == null)
            {
                this.lb_labwares.UnselectAll();
            }

            bool isDoubleClick = e.ClickCount > 1;
            if (isDoubleClick)
                return;

            OnLeftButtonDown(lb_labwares, pt);
        }
        #endregion

        private LayoutEditor GetLayoutEditor()
        {
            //TabItem tabitem = (TabItem)tabDynamic.SelectedItem;
            LayoutEditor layoutEditor = null;
            foreach (TabItem tabItem in tabDynamic.Items)
            {
                layoutEditor = ((Grid)tabItem.Content).Children[0] as LayoutEditor;
                if (layoutEditor != null)
                    break;
            }
            return layoutEditor;
        }

  
    }
}