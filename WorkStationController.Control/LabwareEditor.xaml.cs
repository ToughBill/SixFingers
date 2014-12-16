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
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareEditor : UserControl
    {
        Labware _labware;
        
        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor()
        {
            InitializeComponent();
            this.Loaded += LabwareUserControl_Loaded;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="labware"></param>
        public LabwareEditor(Labware labware):this()
        {
            _labware = labware;
        }

        void LabwareUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Labware labware = this.DataContext as Labware;
            if (labware != null)
            {
                this._colorPicker.SelectedColor = labware.BackgroundColor;
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be LiquidClass
            Labware labware = this.DataContext as Labware;
            if (labware == null)
                throw new InvalidOperationException("DataContext of LabwareEditor must be an instance of Labware");

            // If this instance of labware had been serialized, update it
            string xmlFilePath = string.Empty;
            if (InstrumentsManager.Instance.FindInstrument<Labware>(labware.ID, out xmlFilePath))
            {
                labware.Serialize(xmlFilePath);
            }
            else // else save the instance of labware as a new XML file
            {
                InstrumentsManager.Instance.SaveInstrument<Labware>(labware);
            }
        }
    }
}
