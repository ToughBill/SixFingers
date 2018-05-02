
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareTeachingForm.xaml
    /// </summary>
    public partial class LabwareTeachingForm : Window
    {
        XYZR xyzr = new XYZR(0, 0, 0);
        PositionCalculator positionCalculator;
        InputChecker inputChecker;
        Thread joysThread;
        Thread keyBoardThread;
        DateTime lastUpdateTime = DateTime.Now;
        System.Timers.Timer keepMovingTimer;
        public LabwareTeachingForm()
        {
            InitializeComponent();
            string portNum = ConfigurationManager.AppSettings["PortName"];
            //MoveController.Instance.Init(portNum);
            //MoveController.Instance.MoveHome();
            keepMovingTimer = new System.Timers.Timer(200);
            keepMovingTimer.Elapsed += keepMovingTimer_Elapsed;
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        void keepMovingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (!MoveController.Instance.MoveFinished)
            //    return;
            double x, y, z, r;
            x = y = z = r = 0;
            //MoveController.Instance.GetCurrentPosition(_eARM.左臂, ref x, ref y, ref z, ref r);
            if (xyzr.X != x || xyzr.Y != y || xyzr.Z != z)
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
            //var res = MoveController.Instance.MoveXYZ(_eARM.左臂, xyz.X, xyz.Y, xyz.Z, MoveController.defaultTimeOut);
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            joysThread.Abort();
            keyBoardThread.Abort();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            positionCalculator = new PositionCalculator(xyzr);
            inputChecker = new InputChecker(positionCalculator);
            positionCalculator.OnExpectedPositionChanged += UpdateXYZ;
            joysThread = new Thread(StartJoys);
            joysThread.Start();
            keyBoardThread = new Thread(StartKeyBoard);
            keyBoardThread.Start();
            this.DataContext = xyzr;
        }

        private void Init()
        {
            //get current position
            throw new NotImplementedException();
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
            if(NeedRealMove(DateTime.Now))
            {
                //if (!MoveController.Instance.MoveFinished)
                    return;
                keepMovingTimer.Start();
                MoveXYZ();
                lastUpdateTime = DateTime.Now;
            }
            Debug.WriteLine("X:{0},Y:{1},Z:{2}", xyzr.X, xyzr.Y, xyzr.Z);
        }

        private bool NeedRealMove(DateTime dateTime)
        {
            TimeSpan timeSpan = lastUpdateTime.Subtract(dateTime);
            return timeSpan.Milliseconds > 100; //0.1s
        }

    }
}
