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
            Worktable worktable = new Worktable(
                new Size(6000, 4000),
                new Size(20, 80),
                new Size(20, 120),
                new Size(20, 120), new Point(500, 1000), 1800, 3000, 16);
            Configurations.Instance.Worktable = worktable;
            Container.SizeChanged += Container_SizeChanged;
            WorktableVisual worktableVisual = new WorktableVisual();
            grid1.AttachWorktableVisual(worktableVisual);
            grid1.InvalidateVisual();
            //Labware labware = new Labware();
            //labware.ZValues = new ZValues(360, 625, 665, 1610);
            //labware.Dimension = new Dimension(250, 2800);
            //labware.Name = "lab1";
            //labware.SiteID = 1;
            //labware.Type = LabwareType.Microplates;
            //labware.WellsInfo = new WellsInfo(new Point(0, -32), new Point(0, 2788), 1, 16, BottomShape.Flat, 50);
            //labware.Type = LabwareType.Tubes;
            //labware.BackGroundColor = Color.FromArgb(255, 120, 0, 0);
            //labware.CarrierLabel = "";
            //Container.Children.Add(new LabwareUIElement(labware));
            //Container.RenderSize = new Size(600, 800);
            //VisualCommon.UpdateVisuals(Container.Children);
        }

      

        void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            VisualCommon.UpdateContainerSize(e.NewSize);
            //VisualCommon.UpdateVisuals(Container.Children);
        }

        private void btnNewLabware_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
