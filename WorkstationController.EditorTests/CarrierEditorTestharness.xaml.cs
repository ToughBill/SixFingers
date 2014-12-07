using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkstationController.Control;

namespace WorkstationController.EditorTests
{
    /// <summary>
    /// Interaction logic for CarrierEditorTestharness.xaml
    /// </summary>
    public partial class CarrierEditorTestharness : Window
    {
        CarrierEditor carrierEditor;
        public CarrierEditorTestharness()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CarrierEditorTestharness_Loaded);
        }

        void CarrierEditorTestharness_Loaded(object sender, RoutedEventArgs e)
        {
            carrierEditor = new CarrierEditor(null);
            container.Children.Add(carrierEditor);
        }
    }
}
