using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Control;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.VisualElement;
using WorkstationController.VisualElement.Uitility;

using System;

namespace WorkstationController.EditorTests
{
    /// <summary>
    /// LayoutEditorTestharness.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutEditorTestharness : Window
    {
        RecipeEditor layoutEditor;
        ObservableCollection<WareBase> wares = new ObservableCollection<WareBase>();
        public LayoutEditorTestharness()
        {
            InitializeComponent();
            this.Loaded += LayoutEditorTestharness_Loaded;
        }


        void LayoutEditorTestharness_Loaded(object sender, RoutedEventArgs e)
        {
            layoutEditor = new RecipeEditor();
            //layoutEditor.ContextMenuController.onEditLabware += ContextMenu_onEditLabware;
            UserControlContainer.Children.Add(layoutEditor);
            CreateTwoLabwares();
        }

        void ContextMenu_onEditLabware(object sender, System.EventArgs e)
        {
            //LabwareEditArgs labwareEditArgs = e as LabwareEditArgs;
            //LabwareEditorTestharness labwareEditorTestharness = new LabwareEditorTestharness(labwareEditArgs.Labware);
            //labwareEditorTestharness.Show();
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
            layoutEditor.SuggestCandidate(wares[index]);   
        }

        private void CreateTwoLabwares()
        {
            Labware labware1 = new Labware();
            labware1.ZValues = new ZValues(360, 625, 665, 1610);
            labware1.Dimension = new Dimension(250, 3050);
            labware1.Label = "lab1";
            labware1.SiteID = 1;
            labware1.WellsInfo = new WellsInfo(0, -32, 0, 2788, 1, 16, BottomShape.Flat, 50);
            labware1.TypeName = LabwareBuildInType.Tubes16Pos13_100MM.ToString();
            labware1.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware1.ParentCarrier = null;

            Labware labware2 = new Labware();
            labware2.ZValues = new ZValues(360, 625, 665, 1610);
            labware2.Dimension = new Dimension(1270, 855);
            labware2.Label = "lab2";
            labware2.SiteID = 1;
            labware2.WellsInfo = new WellsInfo(24, 29 ,1014, 609, 12, 8, BottomShape.Flat, 33);
            labware2.TypeName = LabwareBuildInType.Plate96_05ML.ToString();
            labware2.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware2.ParentCarrier = null;

            Labware labware3 = new Labware();
            labware3.ZValues = new ZValues(360, 625, 665, 1610);
            labware3.Dimension = new Dimension(1270, 855);
            labware3.Label = "lab3";
            labware3.SiteID = 1;
            labware3.WellsInfo = new WellsInfo(24, 29,1014, 609, 6, 4, BottomShape.Flat, 60);
            labware3.TypeName = LabwareBuildInType.Plate24_2ML.ToString();
            labware3.BackgroundColor = Color.FromArgb(255, 100,255, 0);
            labware3.ParentCarrier = null;

            Carrier carrier1 = new Carrier(BuildInCarrierType.MP_3POS);
            Carrier carrier2 = new Carrier(BuildInCarrierType.Tube13mm_16POS);
            wares.Add(labware1);
            wares.Add(labware2);
            wares.Add(labware3);
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
            labware.WellsInfo = new WellsInfo(0, -32,0, 2788, 1, 16, BottomShape.Flat, 50);
            labware.TypeName = LabwareBuildInType.Tubes16Pos13_100MM.ToString();
            labware.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware.ParentCarrier = null;
            //layoutEditor.AddCandidate(new LabwareUIElement(labware));
            UserControlContainer.InvalidateVisual();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            layoutEditor.AllowPickup = (bool)chkbox1.IsChecked;
        }
    }
}
