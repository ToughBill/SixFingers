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
using WorkstationController.Core.Utility;
using WTPipetting.Data;
using WTPipetting.Navigation;
using WTPipetting.Utility;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for Protocol.xaml
    /// </summary>
    public partial class LayoutForm : BaseUserControl
    {
        List<Protocol> _protocols;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LayoutForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
            this.Loaded += Protocol_Loaded;
        }

        public PipettorElementManager PipettorElementsManager
        {
            get
            {
                return PipettorElementManager.Instance;
            }
        }


        public List<Protocol> ProtocolNames
        {
            get
            {
                return _protocols;
            }
        }

        void Protocol_Loaded(object sender, RoutedEventArgs e)
        {
            _protocols = ProtocolManager.Instance.Protocols;
            this.DataContext = this;
            lb_layouts.SelectedIndex = 0;
            lstProtocols.SelectedIndex = 0;
        }

        private void lb_layouts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recipeParent.Children.Clear();
            Layout selectedLayout = (Layout)this.lb_layouts.SelectedItem;
            if (selectedLayout == null)
                return;

            LayoutEditor editor = new LayoutEditor(selectedLayout);
            editor.DataContext = selectedLayout;
            recipeParent.Children.Add(editor);
        }

        private void onBtnOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Info("Layout ok pressed.");
                CheckSettings();
            }    
            catch(Exception ex)
            {
                SetInfo(ex.Message, true);
                return;
            }

            NotifyFinished();
        }

        private void SetInfo(string str, bool isError =false)
        {
            txtInfo.Text = str;
            txtInfo.Foreground = isError ? Brushes.Red : Brushes.Black;
        }

        private void CheckSettings()
        {
            SetInfo("");
            if (lb_layouts.SelectedIndex == -1)
            {
                throw new Exception("请选中一个实验布局！");
            }

            if( lstProtocols.SelectedIndex == -1)
            {
                throw new Exception("请选择一个实验方法！");
            }
            int smpCnt = 16;
            bool bInteger = int.TryParse(txtSampleCnt.Text, out smpCnt);
            if (!bInteger)
            {
                throw new Exception("样品数量必须为数字！");
            }

            if (smpCnt < 1)
            {
                throw new Exception("样品数量必须大于0！");
            }

            //check whether labwares exist
            var selectedProtocol = _protocols[lstProtocols.SelectedIndex];
            ProtocolManager.Instance.SelectedProtocol = selectedProtocol;
            Layout selectedLayout = (Layout)this.lb_layouts.SelectedItem;
            ProtocolManager.Instance.CheckLayoutMatchProtocol(selectedLayout, selectedProtocol);
            ProtocolManager.Instance.CheckLiquidExists(PipettorElementManager.Instance.LiquidClasses, selectedProtocol);
            
        }

        private void btnDitiSetting_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Set Diti.");

        }

    }
}
