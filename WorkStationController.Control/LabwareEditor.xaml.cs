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
        PositionCalculator positionCalculator;
        InputChecker inputChecker;
        Thread joysThread;
        Thread keyBoardThread;
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
            if (joysThread.IsAlive)
                joysThread.Abort();
            if (keyBoardThread.IsAlive)
                keyBoardThread.Abort();
        }

        ~LabwareEditor()
        {

        }

        #region teaching implement
        private void keepMovingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TeachingControllerDelegate.Instance.Controller.IsMoving(ArmType.Liha))
                return;
            var currentXYZR = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            if (!xyzr.Equals(currentXYZR))
            {
                MoveXYZ();
            }
            else
            {
                keepMovingTimer.Stop();
            }
        }

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

        private void StartKeyBoard(object obj)
        {
            inputChecker.KeyBoardStart();
        }

        private void StartJoys(object obj)
        {
            inputChecker.JoysStart();
        }

        private void UpdateXYZ(object sender, XYZR e)
        {
            xyzr.X = Math.Round(e.X, 1);
            xyzr.Y = Math.Round(e.Y, 1);
            xyzr.Z = Math.Round(e.Z, 1);
            if (NeedRealMove(DateTime.Now))
            {
                if (TeachingControllerDelegate.Instance.Controller.IsMoving(ArmType.Liha))
                    return;
                keepMovingTimer.Start();
                MoveXYZ();
                lastUpdateTime = DateTime.Now;
            }            
        }

        private bool NeedRealMove(DateTime now)
        {
            TimeSpan timeSpan = now - lastUpdateTime;
            return timeSpan.Milliseconds > 100; //0.1s
        }
        #endregion

        void LabwareEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            labware.UpdateWellInfos();
            joysThread.Abort();
            keyBoardThread.Abort();
            keepMovingTimer.Stop();
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
            keepMovingTimer = new System.Timers.Timer(200);
            keepMovingTimer.Elapsed += keepMovingTimer_Elapsed;
            positionCalculator = new PositionCalculator(xyzr);
            inputChecker = new InputChecker(positionCalculator);
            positionCalculator.OnExpectedPositionChanged += UpdateXYZ;
            joysThread = new Thread(StartJoys);
            joysThread.Start();
            keyBoardThread = new Thread(StartKeyBoard);
            keyBoardThread.Start();
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
            RomaTeachingForm romaTeachingForm = new RomaTeachingForm(labware.PlateVector, positionCalculator,newInfoHandler);
            romaTeachingForm.ShowDialog();
        }



    }
}
