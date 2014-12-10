using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.VisualElement.contextMenu;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ContextMenuForm : Window
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ContextMenuForm()
        {
            InitializeComponent();
            this.Loaded += ContextMenuForm_Loaded;
        }

        public ObservableCollection<ContextMenuEntity> MenuEntities { get; set; }


        public void SetMenus(ObservableCollection<ContextMenuEntity> menuEntities)
        {
            MenuEntities = menuEntities;
            //lstContextMenus.ItemsSource = MenuEntities;
            //lstContextMenus.ItemsSource = _menuEntities;
        }

        void ContextMenuForm_Loaded(object sender, RoutedEventArgs e)
        {
            lstContextMenus.MouseDown += lstContextMenus_MouseDown;
        }

        void lstContextMenus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int selIndex = lstContextMenus.SelectedIndex;
            if (selIndex == -1)
                return;
            var command = MenuEntities[selIndex].Command2Do;
            command.Execute(null,this);
        }
    }
}
