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
        System.Timers.Timer updatePositionTimer;
        
        WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler;
        public RomaTeachingForm(PlateVector plateVector,System.Timers.Timer  updatePositionTimer,
            WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler)
        {
            InitializeComponent();
            this.plateVector = plateVector;
            this.newInfoHandler = newInfoHandler;
            this.updatePositionTimer = updatePositionTimer;
            updatePositionTimer.Elapsed += updatePositionTimer_Elapsed;
            this.Loaded += RomaTeachingForm_Loaded;
        }

        void updatePositionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Roma);
            plateVector.CurrentPosition.X = xyzr.X;
            plateVector.CurrentPosition.Y = xyzr.Y;
            plateVector.CurrentPosition.Z = xyzr.Z;
            plateVector.CurrentPosition.R = xyzr.R;

        }

     
        void RomaTeachingForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = plateVector;
            if (plateVector.CurrentPosition == null)
                plateVector.CurrentPosition = plateVector.Positions[0];
            
        }
        private void UpdateXYZR(object sender, XYZR e)
        {
            plateVector.CurrentPosition.X = xyzr.X = Math.Round(e.X, 1);
            plateVector.CurrentPosition.Y = xyzr.Y = Math.Round(e.Y, 1);
            plateVector.CurrentPosition.Z = xyzr.Z = Math.Round(e.Z, 1);
            plateVector.CurrentPosition.R = xyzr.R = Math.Round(e.R, 1);
           
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
