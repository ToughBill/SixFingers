using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.VisualElement;
using WorkstationController.Core.Data;

namespace WorkstationController.WorktableVisualTest
{
    /// <summary>
    /// MainWindow.xaml logic
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Labware labware1 = new Labware();
            labware1.ZValues = new ZValues(360, 625, 665, 1610);
            labware1.Dimension = new Dimension(1270,855);
            labware1.TypeName = "16Pos Tubes";
            labware1.Label = "lab1";
            labware1.SiteID = 1;
            labware1.WellsInfo = new WellsInfo(144, 115,990+144, 745, 12, 8, BottomShape.Flat, 40);
            labware1.TypeName = LabwareBuildInType.Plate96_05ML.ToString();
            labware1.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware1.ParentCarrier = null;

            LabwareUIElementFixedSize labwareUIElement = new LabwareUIElementFixedSize(labware1, new Size(400, 400));
            grid1.Children.Add(labwareUIElement);
             
            this.MouseMove += MainWindow_MouseMove;
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            //(grid1.Children[0] as LabwareUIElementFixedSize).UpdateMousePosition(e);
            //grid1.InvalidateVisual();
        }
    }
}
