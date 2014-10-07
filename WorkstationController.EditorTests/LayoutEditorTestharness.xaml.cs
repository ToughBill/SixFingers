using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Control;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.EditorTests
{
    /// <summary>
    /// LayoutEditorTestharness.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutEditorTestharness : Window
    {
        LayoutEditor layoutEditor;
        ObservableCollection<WareBase> wares = new ObservableCollection<WareBase>();
        public LayoutEditorTestharness()
        {
            InitializeComponent();
            this.Loaded += LayoutEditorTestharness_Loaded;
        }

        void LayoutEditorTestharness_Loaded(object sender, RoutedEventArgs e)
        {
            layoutEditor = new LayoutEditor();
            UserControlContainer.Children.Add(layoutEditor);
            CreateTwoLabwares();
        }

        private void lstboxLabwares_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(lstboxLabwares);
            HitTestResult result = VisualTreeHelper.HitTest(lstboxLabwares, pt);
            ListBoxItem lbi = VisualCommon.FindParent<ListBoxItem>(result.VisualHit);
            int index = lstboxLabwares.ItemContainerGenerator.IndexFromContainer(lbi);
            if (index == -1)
                return;
            layoutEditor.AddCandidate(UIElementFactory.CreateUIElement(wares[index]));
            
        }

        private void CreateTwoLabwares()
        {
            Labware labware1 = new Labware();
            labware1.ZValues = new ZValues(360, 625, 665, 1610);
            labware1.Dimension = new Dimension(250, 2700);
            labware1.TypeName = "16Pos Tubes";
            labware1.Label = "lab1";
            labware1.SiteID = 1;
            labware1.WellsInfo = new WellsInfo(new Point(0, -32), new Point(0, 2788), 1, 16, BottomShape.Flat, 50);
            labware1.TypeName = "Tubes 16Pos 100mm";
            labware1.BackGroundColor = Color.FromArgb(255, 255, 0, 0);
            labware1.CarrierGrid = 0;

            Carrier carrier1 = new Carrier(BuildInCarrierType.MP_3POS);
            carrier1.Grid = 2;
            //Labware labware2 = new Labware();
            //labware2.ZValues = new ZValues(360, 625, 665, 1610);
            //labware2.Dimension = new Dimension(250, 2700);
            //labware2.TypeName = "16Pos Tubes";
            //labware2.Label = "lab2";
            //labware2.SiteID = 1;
            //labware2.WellsInfo = new WellsInfo(new Point(0, -32), new Point(0, 2788), 1, 4, BottomShape.Flat, 50);
            //labware2.LabwareType = LabwareType.Tubes;
            //labware2.BackGroundColor = Color.FromArgb(255, 255, 0, 0);
            //labware2.CarrierLabel = "";
            wares.Add(labware1);
            wares.Add(carrier1);
            lstboxLabwares.ItemsSource = wares;
        }

        private void btnNewLabware_Click(object sender, RoutedEventArgs e)
        {
            Labware labware = new Labware();
            labware.ZValues = new ZValues(360, 625, 665, 1610);
            labware.Dimension = new Dimension(240, 3050);
            labware.TypeName = "lab1";
            labware.SiteID = 1;
            labware.WellsInfo = new WellsInfo(new Point(0, -32), new Point(0, 2788), 1, 16, BottomShape.Flat, 50);
            labware.TypeName = LabwareBuildInType.Tubes16Pos13_100MM.ToString();
            labware.BackGroundColor = Color.FromArgb(255, 255, 0, 0);
            labware.CarrierGrid = 1;
            //layoutEditor.AddCandidate(new LabwareUIElement(labware));
            UserControlContainer.InvalidateVisual();
        }

 

    }
}
