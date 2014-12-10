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

        private ObservableCollection<ContextMenuEntity> MenuEntities { get; set; }


        public void SetMenus(ObservableCollection<ContextMenuEntity> menuEntities)
        {
            MenuEntities = menuEntities;
        }

        void ContextMenuForm_Loaded(object sender, RoutedEventArgs e)
        {
            lstContextMenus.PreviewMouseLeftButtonDown += lstContextMenus_PreviewMouseLeftButtonDown; ;
        }

        void lstContextMenus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(lstContextMenus, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item == null)
                return;
            string s = item.Content.ToString().ToLower();
            var command = MenuEntities.Where(x => x.Text.ToLower() == s).Select(x => x.Command2Do).ToList().First();
            command.Execute(null, this);
        }
    }
}
