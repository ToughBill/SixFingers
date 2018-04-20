using MoveControl;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MoveControlForm.xaml
    /// </summary>
    public partial class MoveControlForm : Window
    {
        XYZ xyz = new XYZ(0, 0, 0);
        PositionCalculator positionCalculator;
        InputChecker inputChecker;
        Thread joysThread;
        Thread keyBoardThread;
        public MoveControlForm()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            joysThread.Abort();
            keyBoardThread.Abort();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            positionCalculator = new PositionCalculator(xyz);
            inputChecker = new InputChecker(positionCalculator);
            positionCalculator.OnPositionChanged += UpdateXYZ;
            joysThread = new Thread(StartJoys);
            joysThread.Start();
            keyBoardThread = new Thread(StartKeyBoard);
            keyBoardThread.Start();
            this.DataContext = xyz;
        }

        private void Init()
        {
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
       
        private void UpdateXYZ(object sender, XYZ e)
        {
            xyz.X = Math.Round(e.X, 1);
            xyz.Y = Math.Round(e.Y, 1);
            xyz.Z = Math.Round(e.Z, 1);
            Debug.WriteLine("X:{0},Y:{1},Z:{2}", xyz.X, xyz.Y, xyz.Z);
        }

    }
}
