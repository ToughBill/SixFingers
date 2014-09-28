using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.Core.Data;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareUserControl : UserControl
    {
        public LabwareUserControl()
        {
            InitializeComponent();
            this.Loaded += LabwareUserControl_Loaded;
        }

        void LabwareUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbItemType.ItemsSource = Enum.GetValues(typeof(LabwareType)).Cast<LabwareType>();
        }
    }
}
