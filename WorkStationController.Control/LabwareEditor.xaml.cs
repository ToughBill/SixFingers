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
        XYZ xyz = new XYZ(0, 0, 0);
        RomaTeachingForm romaTeachingForm;
        InputChecker inputChecker = new InputChecker();
        DateTime lastUpdateTime = DateTime.Now;
        System.Timers.Timer updatePositionTimer = new System.Timers.Timer(250);
        public event EventHandler<ROMAPosition> onPositionChanged;
        int speed = 1;
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor(NewInformationHandler newInfoHandler,Window parent)
            :base(newInfoHandler)
        {
            InitializeComponent();
            inputChecker.OnStartMove += inputChecker_OnStartMove;
            inputChecker.OnStopMove += inputChecker_OnStopMove;
            TeachingControllerDelegate.Instance.RegisterController(new TeachingControllerSimulator());
            this.Loaded += LabwareEditor_Loaded;
            this.Unloaded += LabwareEditor_Unloaded;
            parent.Closing += parent_Closing;
        }

       

        void parent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            inputChecker.Stop();
        }

        ~LabwareEditor()
        {
        }

        #region teaching implement
        private void GetCurrentPositon()
        {
            //get current position
            ArmType armType = ArmType.Liha;

            if( romaTeachingForm != null && romaTeachingForm.IsVisible)
            {
                armType = ArmType.Roma;
            }
            var position = TeachingControllerDelegate.Instance.Controller.GetPosition(armType);
            double degree, clipWidth;
            degree = clipWidth = 0;
            if(armType == ArmType.Roma)
            {
                TeachingControllerDelegate.Instance.Controller.GetClipperInfo(ref degree, ref clipWidth); 
                if (onPositionChanged != null)
                    onPositionChanged(this, new ROMAPosition("x", position.X, position.Y, position.Z, degree, clipWidth));
                return;
            }
            

            Debug.WriteLine("Current Position is: xyz {0}{1}{2}", position.X, position.Y, position.Z);
            xyz.X = position.X;
            xyz.Y = position.Y;
            xyz.Z = position.Z;
         
        }

        private void StartCheckingInput()
        {
            inputChecker.Start();
            updatePositionTimer.Start();
        }

        void inputChecker_OnStopMove(object sender, Direction e)
        {
            Debug.WriteLine("input checker on stopp move");
            TeachingControllerDelegate.Instance.Controller.StopMove();
            updatePositionTimer.Stop();
            GetCurrentPositon();

        }

        void inputChecker_OnStartMove(object sender, Direction dir)
        {
            ArmType armType = ArmType.Liha;
            if (romaTeachingForm != null && romaTeachingForm.IsVisible)
            {
                armType = ArmType.Roma;
            }
            TeachingControllerDelegate.Instance.Controller.StartMove(armType, dir, speed);
            updatePositionTimer.Start();
        }

        #endregion
        void LabwareEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            labware.UpdateWellInfos();
            inputChecker.Stop();
            updatePositionTimer.Stop();
        }

        void LabwareEditor_Loaded(object sender, RoutedEventArgs e)
        {
            //改成循环测试，直到连接上，或者放弃StartCheckingInput
            try
            {
                TeachingControllerDelegate.Instance.Controller.Init();
            }
            catch(CriticalException ex)
            {
                if(ex.Message == "Send_fail")
                {
                    MessageBox.Show("初始化失败，可能是机器未通电");
                }
            }

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
            GetCurrentPositon();
            curPositionPanel.DataContext = xyz;
            StartCheckingInput();
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be labware
            if (labware == null)
                throw new InvalidOperationException("DataContext of LabwareEditor must be an instance of Labware");

            
            labware.UpdateWellInfos();
            try
            {
                CheckValidZValues();
                PipettorElementManager.Instance.SavePipettorElement(labware);
            }
            catch(Exception ex)
            {
                if (newInfoHandler != null)
                    newInfoHandler(ex.Message, true);
            }
        }

        private void CheckValidZValues()
        {
            CheckZValue(labware.ZValues.ZTravel, "ZTravel");
            CheckZValue(labware.ZValues.ZStart, "ZStart");
            CheckZValue(labware.ZValues.ZDispense, "ZDispense");
            CheckZValue(labware.ZValues.ZMax, "ZMax");
            
        }

        private void CheckZValue(float value, string description)
        {
            if (value > TeachingControllerDelegate.Instance.Controller.ZMax)
                throw new Exception(string.Format("{0}不得大于{1}", description, TeachingControllerDelegate.Instance.Controller.ZMax));

            if (value < 0)
                throw new Exception(string.Format("{0}不得小于0", description));
            
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
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionLastWell_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.X = labware.BottomRightWellX;
            xyzr.Y = labware.BottomRightWellY;
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZTravel_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZTravel;
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZMax_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZMax;
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZDispense_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZDispense;
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }

        private void btnMove2CurrentPositionZStart_Click(object sender, RoutedEventArgs e)
        {
            var xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            xyzr.Z = labware.ZValues.ZStart;
            TeachingControllerDelegate.Instance.Controller.Move2XYZ(ArmType.Liha, xyzr);
        }
        #endregion

        private void OnSetROMAVectorClick(object sender, RoutedEventArgs e)
        {
            if (labware.PlateVector == null)
                labware.PlateVector = new PlateVector(true);
            labware.PlateVector.Name = labware.Label;
            romaTeachingForm = new RomaTeachingForm(labware.PlateVector,this,newInfoHandler);
            romaTeachingForm.ShowDialog();
        }

        private void rdbSpeed_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbHighSpeed == null)
                return;

            bool isLowSpeed = (bool)rdbLowSpeed.IsChecked;
            bool isMediumSpeed = (bool)rdbMediumSpeed.IsChecked;
            bool isHighSpeed = (bool)rdbHighSpeed.IsChecked;
            if (isLowSpeed)
                speed = 1;
            else if (isMediumSpeed)
                speed = 10;
            else
                speed = 50;
        }

        private void btnGetTip_Click(object sender, RoutedEventArgs e)
        {
            TeachingControllerDelegate.Instance.Controller.GetTip();
        }

        private void btnDropTip_Click(object sender, RoutedEventArgs e)
        {
            TeachingControllerDelegate.Instance.Controller.DropTip();
        }
    }
}
