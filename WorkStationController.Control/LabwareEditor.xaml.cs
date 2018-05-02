using System;
using System.Windows;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.Hardware;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareEditor : BaseEditor
    {
        Labware labware;
        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor(NewInformationHandler newInfoHandler)
            :base(newInfoHandler)
        {
            InitializeComponent();
            this.Loaded += LabwareEditor_Loaded;
            this.Unloaded += LabwareEditor_Unloaded;
        }
   
        void LabwareEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            labware.UpdateWellInfos();
        }

        void LabwareEditor_Loaded(object sender, RoutedEventArgs e)
        {
            labware = this.DataContext as Labware;
            int index = -1;
            if(labware.ParentCarrier != null)
            {
                int tempIndex = 0;
                foreach(var carrier in labware.AllCarriers)
                {
                    if(carrier.TypeName == labware.ParentCarrier.TypeName)
                    {
                        index = tempIndex;
                        break;
                    }
                }
                if (index != -1)
                    cmbCalibCarrier.SelectedIndex = index;
            }
            labware.CalculatePositionInLayout();
            
        }


        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be labware
            if (labware == null)
                throw new InvalidOperationException("DataContext of LabwareEditor must be an instance of Labware");
            labware.UpdateWellInfos();
            try
            {
                PipettorElementManager.Instance.SavePipettorElement(labware);
            }
            catch(Exception ex)
            {
                if (newInfoHandler != null)
                    newInfoHandler(ex.Message, true);
            }
            
        }

        private void OnMoveControlBtnClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnUseCurrentValFirtWell_Click(object sender, RoutedEventArgs e)
        {
           var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
           labware.TopLeftWellX = xyzr.X;
           labware.TopLeftWellY = xyzr.Y;
        }

        private void btnUseCurrentValLastWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
            labware.BottomRightWellX = xyzr.X;
            labware.BottomRightWellY = xyzr.Y;
        }

        private void btnUseCurrentValZTravel_Click(object sender, RoutedEventArgs e)
        {
           
            labware.ZValues.ZTravel = GetZValue(); 

        }

        private float GetZValue()
        {
            var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
            return (float)xyzr.Z;
        }

        private void btnUseCurrentValZStart_Click(object sender, RoutedEventArgs e)
        {
            labware.ZValues.ZStart = GetZValue(); 
        }

        private void btnUseCurrentValZDispense_Click(object sender, RoutedEventArgs e)
        {
            labware.ZValues.ZDispense = GetZValue(); 
        }

        private void btnUseCurrentValZMax_Click(object sender, RoutedEventArgs e)
        {
            labware.ZValues.ZMax = GetZValue(); 
        }

        private void btnMove2CurrentPositionFirstWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
            xyzr.X = labware.TopLeftWellX;
            xyzr.Y = labware.TopLeftWellY;
            TeachingDelegate.Instance.Worker.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionLastWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
            xyzr.X = labware.BottomRightWellX;
            xyzr.Y = labware.BottomRightWellY;
            TeachingDelegate.Instance.Worker.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZTravel_Click(object sender, RoutedEventArgs e)
        {
            //var xyzr = TeachingDelegate.Instance.Worker.GetPosition(ArmType.Liha);
            //xyzr.X = labware.BottomRightWellX;
            //xyzr.Y = labware.BottomRightWellY;
            //TeachingDelegate.Instance.Worker.Move2XYZR(ArmType.Liha, xyzr);
        }

        
    }
}
