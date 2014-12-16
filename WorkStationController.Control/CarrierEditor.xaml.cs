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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for CarrierEditor.xaml
    /// </summary>
    public partial class CarrierEditor : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CarrierEditor()
        {
            InitializeComponent();
            this.Loaded += CarrierEditor_Loaded;
        }

        void CarrierEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            if (carrier != null)
            {
                this._colorPicker.SelectedColor = carrier.BackgroundColor;
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be Carrier
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");

            // If this instance of LiquidClass had been serialized, save it
            string xmlFilePath = string.Empty;
            if (InstrumentsManager.Instance.FindInstrument<Carrier>(carrier.ID, out xmlFilePath))
            {
                carrier.Serialize(xmlFilePath);
            }
            else // else save the instance of LiquidClass as a new XML file
            {
                InstrumentsManager.Instance.SaveInstrument<Carrier>(carrier);
            }
        }

        private void btnAddSite_Click(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");
            carrier.Sites.Add(new Site());
        }

        private void btnRemoveSite_Click(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");
            if (carrier.Sites.Count == 0)
                return;
            carrier.Sites.RemoveAt(0);
        }
    }
}
