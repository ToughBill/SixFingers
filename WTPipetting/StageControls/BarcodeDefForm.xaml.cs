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
using WTPipetting.Navigation;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for BarcodeDefForm.xaml
    /// </summary>
    public partial class BarcodeDefForm :  BaseUserControl
    {
        public BarcodeDefForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Barcode confirmed");
            NotifyFinished();
        }
    }
}
