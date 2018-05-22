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
        XYZ xyzr;
        PlateVector plateVector;
        DateTime lastUpdateTime = DateTime.Now;
        System.Timers.Timer updatePositionTimer;
        
        WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler;
        public RomaTeachingForm(PlateVector plateVector, LabwareEditor labwareEditor,
            WorkstationController.Control.BaseEditor.NewInformationHandler newInfoHandler)
        {
            InitializeComponent();
            this.plateVector = plateVector;
            this.newInfoHandler = newInfoHandler;
            labwareEditor.onPositionChanged += labwareEditor_onPositionChanged;
            this.Loaded += RomaTeachingForm_Loaded;
        }

        void labwareEditor_onPositionChanged(object sender, ROMAPosition e)
        {
            plateVector.CurrentPosition.X = e.X;
            plateVector.CurrentPosition.Y = e.Y;
            plateVector.CurrentPosition.Z = e.Z;
            plateVector.CurrentPosition.R = e.R;
            plateVector.CurrentPosition.ClipDistance = e.ClipDistance;
        }

     
        void RomaTeachingForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = plateVector;
            if (plateVector.CurrentPosition == null)
                plateVector.CurrentPosition = plateVector.Positions[0];
            
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
