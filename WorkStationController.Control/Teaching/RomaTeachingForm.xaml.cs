using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.Hardware;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for RomaTeachingForm.xaml
    /// </summary>
    public partial class RomaTeachingForm : Window
    {
        XYZR xyzr;
        PlateVector plateVector;
        DateTime lastUpdateTime = DateTime.Now;
        System.Timers.Timer keepMovingTimer;
        
        WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler;
        public RomaTeachingForm(PlateVector plateVector, PositionCalculator positionCalculator,
            WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler)
        {
            InitializeComponent();
            this.plateVector = plateVector;
            this.newInfoHandler = newInfoHandler;
            positionCalculator.OnExpectedPositionChanged += UpdateXYZR;
            this.Loaded += RomaTeachingForm_Loaded;
        }

     
        void RomaTeachingForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = plateVector;
            xyzr = new XYZR(0, 0, 0);
            xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Roma);
            if (plateVector.CurrentPosition == null)
                plateVector.CurrentPosition = plateVector.Positions[0];
            keepMovingTimer = new System.Timers.Timer(200);
            keepMovingTimer.Elapsed += keepMovingTimer_Elapsed;
        }
        private void UpdateXYZR(object sender, XYZR e)
        {
            plateVector.CurrentPosition.X = xyzr.X = Math.Round(e.X, 1);
            plateVector.CurrentPosition.Y = xyzr.Y = Math.Round(e.Y, 1);
            plateVector.CurrentPosition.Z = xyzr.Z = Math.Round(e.Z, 1);
            plateVector.CurrentPosition.R = xyzr.R = Math.Round(e.R, 1);
            if (NeedRealMove(DateTime.Now))
            {
                if (TeachingControllerDelegate.Instance.Controller.IsMoving(ArmType.Roma))
                    return;
                keepMovingTimer.Start();
                MoveXYZR();
                lastUpdateTime = DateTime.Now;
            }
        }

        private void MoveXYZR()
        {
            try
            {
                TeachingControllerDelegate.Instance.Controller.Move2XYZR(ArmType.Roma, xyzr);
            }
            catch (Exception ex)
            {
                newInfoHandler(ex.Message, true);
            }
        }

        private bool NeedRealMove(DateTime now)
        {
            TimeSpan timeSpan = now - lastUpdateTime;
            return timeSpan.Milliseconds > 100; //0.1s
        }
        private void keepMovingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (TeachingControllerDelegate.Instance.Controller.IsMoving(ArmType.Liha))
                return;
            var currentXYZR = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            if (!xyzr.Equals(currentXYZR))
            {
                MoveXYZR();
            }
            else
            {
                keepMovingTimer.Stop();
            }
        }

       

        private void btnAddPosition_Click(object sender, RoutedEventArgs e)
        {
            plateVector.AddNewPosition();
        }

        private void btnDeletePosition_Click(object sender, RoutedEventArgs e)
        {

            if (plateVector.Positions.Count == 0)
                return;
            if (plateVector.CurrentPosition != null)
                plateVector.RemoveCurrent();

        }

       

    }
}
