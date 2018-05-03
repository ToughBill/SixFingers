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
        PlateVector plateVector;
        DateTime lastUpdateTime = DateTime.Now;
        XYZR xyzr;
        public RomaTeachingForm(PlateVector plateVector,PositionCalculator positionCalculator)
        {
            InitializeComponent();
            this.plateVector = plateVector;
            positionCalculator.OnExpectedPositionChanged += UpdateXYZR;
            this.Loaded += RomaTeachingForm_Loaded;
        }

     
        void RomaTeachingForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = plateVector;
            //xyzr = new XYZR(0, 0, 0);
            //xyzr = TeachingControllerDelegate.Instance.Controller.GetPosition(ArmType.Liha);
            //currentPosPanel.DataContext = xyzr;
        }

        

        private void UpdateXYZR(object sender, XYZR e)
        {
            plateVector.CurrentPosition.X = Math.Round(e.X, 1);
            plateVector.CurrentPosition.Y = Math.Round(e.Y, 1);
            plateVector.CurrentPosition.Z = Math.Round(e.Z, 1);
            plateVector.CurrentPosition.R = Math.Round(e.R, 1);
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

        private void btnUseCurrentVal_Click(object sender, RoutedEventArgs e)
        {
            string id = plateVector.CurrentPosition.ID;
            plateVector.CurrentPosition = new ROMAPosition(id, xyzr.X, xyzr.Y, xyzr.Z, xyzr.R);
        }

    }
}
