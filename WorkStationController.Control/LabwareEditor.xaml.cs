using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.Hardware;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareEditor : BaseEditor
    {
        Labware labware;
        #region teaching related
        XYZR xyzr = new XYZR(0, 0, 0);
      
        InputChecker inputChecker;
        Thread inputCheckingThread;
      
        DateTime lastUpdateTime = DateTime.Now;
        System.Timers.Timer keepMovingTimer;
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor(NewInformationHandler newInfoHandler,Window parent)
            :base(newInfoHandler)
        {
            InitializeComponent();
         
            this.Loaded += LabwareEditor_Loaded;
            this.Unloaded += LabwareEditor_Unloaded;
            parent.Closing += parent_Closing;
        }

        void parent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (inputCheckingThread.IsAlive)
                inputCheckingThread.Abort();
         
        }

        ~LabwareEditor()
        {

        }

        #region teaching implement
  

        private void MoveXYZ()
        {
            try
            {
                TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
            }
            catch (Exception ex)
            {
                newInfoHandler(ex.Message, true);
            }
        }


        private void Init()
        {
            //get current position
            xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
        }

    

        private void StartCheckingInput(object obj)
        {
            inputChecker.Start();
        }

    

      
        #endregion

        void LabwareEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            labware.UpdateWellInfos();
            inputCheckingThread.Abort();
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

            Init();
            inputChecker = new InputChecker();
            inputChecker.Start();
            curPositionPanel.DataContext = xyzr;
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

      
     

        #region 使用当前位置，移动到设置位置
        private void btnUseCurrentValFirtWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            labware.TopLeftWellX = xyzr.X;
            labware.TopLeftWellY = xyzr.Y;
        }

        private void btnUseCurrentValLastWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            labware.BottomRightWellX = xyzr.X;
            labware.BottomRightWellY = xyzr.Y;
        }

        private void btnUseCurrentValZTravel_Click(object sender, RoutedEventArgs e)
        {
            labware.ZValues.ZTravel = GetZValue();

        }

        private float GetZValue()
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
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
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.X = labware.TopLeftWellX;
            xyzr.Y = labware.TopLeftWellY;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionLastWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.X = labware.BottomRightWellX;
            xyzr.Y = labware.BottomRightWellY;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZTravel_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZTravel;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZMax_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZMax;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZDispense_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZDispense;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZStart_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZStart;
            TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Liha, xyzr);
        }

        #endregion

        private void OnSetROMAVectorClick(object sender, RoutedEventArgs e)
        {
            if (labware.PlateVector == null)
                labware.PlateVector = new PlateVector(true);
            labware.PlateVector.Name = labware.Label;
            RomaTeachingForm romaTeachingForm = new RomaTeachingForm(labware.PlateVector, newInfoHandler);
            romaTeachingForm.ShowDialog();
        }



    }
}
