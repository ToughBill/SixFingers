using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.Core.Data;

namespace WorkstationController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Labware> _labwares = new ObservableCollection<Labware>();
        ObservableCollection<Carrier> _carriers = new ObservableCollection<Carrier>();
        ObservableCollection<Layout> _layouts = new ObservableCollection<Layout>();
        ObservableCollection<LiquidClass> _liquidClasses = new ObservableCollection<LiquidClass>();

        private List<TabItem> _tabItems = new List<TabItem>();
        private TabItem _tabAdd = new TabItem();

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // add a tabItem with + in header 
            _tabAdd.Header = "+";
            _tabItems.Add(_tabAdd);
            this.tabDynamic.DataContext = _tabItems;
        }

        #region Dynamic TabItem
        private TabItem AddTabItem()
        {
            int count = _tabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();

            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            // add controls to tab item, this case I added just a textbox
            TextBox txt = new TextBox();
            txt.Name = "txt";

            tab.Content = txt;

            // insert tab item right before the last (+) tab item
            _tabItems.Insert(count - 1, tab);

            return tab;
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabDynamic.SelectedItem as TabItem;
            if (tab == null) return;

            // Click "+" tab to add a new tab
            if (tab.Equals(_tabAdd))
            {
                // clear tab control binding
                tabDynamic.DataContext = null;

                TabItem newTab = this.AddTabItem();

                // bind tab control
                tabDynamic.DataContext = _tabItems;

                // select newly added tab item
                tabDynamic.SelectedItem = newTab;
            }
            else
            {
                // your code here...
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

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
                if (selectedTab == null || selectedTab.Equals(tab))
                {
                    selectedTab = _tabItems[0];
                }
                tabDynamic.SelectedItem = selectedTab;
            }
        }
        #endregion
    }
}
