using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkstationController.Control;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.EditorTests
{
    /// <summary>
    /// PipettingEditorTestharness.xaml 的交互逻辑
    /// </summary>
    public partial class PipettingEditorTestharness : Window
    {
        public PipettingEditorTestharness()
        {
            InitializeComponent();
            this.Loaded += PipettingEditorTestharness_Loaded;
        }

        void PipettingEditorTestharness_Loaded(object sender, RoutedEventArgs e)
        {
            Labware labware1 = new Labware();
            labware1.ZValues = new ZValues(360, 625, 665, 1610);
            labware1.Dimension = new Dimension(127, 85.5);
            labware1.TypeName = "16Pos Tubes";
            labware1.Label = "lab1";
            labware1.SiteID = 1;
            labware1.WellsInfo = new WellsInfo(14.4, 11.5, 99 + 14.4, 74.5, 12, 8, BottomShape.Flat, 4);
            labware1.TypeName = LabwareBuildInType.Plate96_05ML.ToString();
            labware1.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware1.ParentCarrier = null;

            container.Children.Add(new PipettingCommandEidtor(labware1,false));
        }

        public void UpdateLabware(Labware newLabware)
        {
            container.Children.Clear();
            container.Children.Add(new PipettingCommandEidtor(newLabware, false));
        }
    }
}
