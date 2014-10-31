using System;
using System.Collections.Generic;
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
using WorkstationController.Control;
using WorkstationController.Core.Data;

namespace WorkstationController.EditorTests
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string LabwareEditor = "LabwareEditor";
        const string CarrierEditor = "CarrierEditor";
        UserControl labwareEditorUserControl;
        UserControl carrierEditorUserControl;
        Labware labware = new Labware();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> strs = new List<string>();
            strs.Add(LabwareEditor);
            strs.Add(CarrierEditor);
            labwareEditorUserControl = new LabwareEditor();
            carrierEditorUserControl = new CarrierEditor();
          
            labware.ZValues = new ZValues(360, 625, 665, 1610);
            labware.Dimension = new Dimension(0, 2700);
            labware.TypeName = "lab1";
            labware.SiteID = 1;
            labware.TypeName = LabwareBuildInType.Tubes16Pos13_100MM.ToString();
            labware.WellsInfo = new WellsInfo(0, -32, 0, 2788, 1, 16, BottomShape.Flat, 50);
            labware.BackgroundColor = Color.FromArgb(255, 255, 0, 0);
            labware.ParentCarrier = null;

            this.DataContext = labware;
            UserControlNames.ItemsSource = strs;
        }

        private void UserControlNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sUserCOntrolName = UserControlNames.SelectedItem.ToString();
            switch(sUserCOntrolName)
            {
                case LabwareEditor:
                    UserControlContainer.Children.Clear();
                    UserControlContainer.Children.Add(labwareEditorUserControl);
                    break;
                case CarrierEditor:
                    UserControlContainer.Children.Clear();
                    //EditorContainer.Children.Add(carrierEditorUserControl);
                    break;
                default:
                    break;
            }
        }

        private void btnSave2Xml_Click(object sender, RoutedEventArgs e)
        {
            labware.Serialize("d:\\test.xml");
        }
    }
}
