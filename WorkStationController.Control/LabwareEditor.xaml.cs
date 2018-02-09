using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
using WorkstationController.Core;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareEditor : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor()
        {
            InitializeComponent();
            this.Loaded += LabwareEditor_Loaded;
            
        }

       

        void LabwareEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Labware labware = this.DataContext as Labware;
            var calibCarrier = labware.CalibCarrier;
            int currentIndex = -1;
            for(int i = 0; i< labware.AllCarriers.Count; i++)
            {
                if(labware.AllCarriers[i].TypeName == calibCarrier.TypeName)
                {
                    currentIndex = i;
                    break;
                }
            }
            cmbCalibCarrier.SelectedIndex = currentIndex;
        }


        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be labware
            Labware labware = this.DataContext as Labware;
            labware.UpdateWellInfos();
            if (labware == null)
                throw new InvalidOperationException("DataContext of LabwareEditor must be an instance of Labware");
            PipettorElementManager.Instance.SavePipettorElement(labware);
        }
    }
}
