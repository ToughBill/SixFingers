using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Control;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;
using WorkstationController.VisualElement.Uitility;

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
            if (lbi == null)
                return;
            int index = lstboxLabwares.ItemContainerGenerator.IndexFromContainer(lbi);
            if (index == -1)
                return;
            layoutEditor.SuggestCandidate(UIElementFactory.CreateUIElement(wares[index]));
            
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
            labware1.TypeName = LabwareBuildInType.Tubes16Pos13_100MM.ToString();
            labware1.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware1.CarrierGrid = 0;

            Carrier carrier1 = new Carrier(BuildInCarrierType.MP_3POS);
            Carrier carrier2 = new Carrier(BuildInCarrierType.Tube13mm_16POS);
            wares.Add(labware1);
            wares.Add(carrier1);
            wares.Add(carrier2);
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
            labware.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware.CarrierGrid = 1;
            //layoutEditor.AddCandidate(new LabwareUIElement(labware));
            UserControlContainer.InvalidateVisual();
        }

 

    }
}
