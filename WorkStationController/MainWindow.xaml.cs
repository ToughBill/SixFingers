﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WorkstationController.Control;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private members
        private InstrumentsManager _instrumentsManager = InstrumentsManager.Instance;

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
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this._instrumentsManager.Initialize();

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
            tab.Tag = Guid.NewGuid();
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            StackPanel stackpanel = new StackPanel();
            stackpanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            stackpanel.Children.Add(control);
            tab.Content = stackpanel;

            // insert tab at most left position
            _tabItems.Insert(0, tab);

            tabDynamic.DataContext = null;
            tabDynamic.DataContext = _tabItems;
            tabDynamic.SelectedItem = tab;

            return tab;
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Guid tabName = (Guid)(sender as Button).CommandParameter;

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Tag.Equals(tabName)).SingleOrDefault();

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
        #endregion

        #region Labware context menu
        private void OnLabwareEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            LabwareEditor editor = new LabwareEditor();
            Labware selectedLW = (Labware)this.lb_labwares.SelectedItem;
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
        }
        #endregion

        #region Carrier context menu
        private void OnCarrierEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            CarrierEditor editor = new CarrierEditor();
            Carrier selectedCr = (Carrier)this.lb_carriers.SelectedItem;
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
            Carrier selectedCr = (Carrier)this.lb_labwares.SelectedItem;
            InstrumentsManager.Instance.DeleteInstrument<Carrier>(selectedCr.ID);
        }
        #endregion

        #region LiquidClass context menu
        private void OnLiquidClassEditMenuItemClick(object sender, RoutedEventArgs e)
        {
            LiquidClassEditor editor = new LiquidClassEditor();
            LiquidClass selectedLC = (LiquidClass)this.lb_liquidclass.SelectedItem;
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
        }
        #endregion
    }
}
