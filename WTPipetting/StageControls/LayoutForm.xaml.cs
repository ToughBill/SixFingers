﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
using WorkstationController.Hardware;
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
            string sPort = ConfigurationManager.AppSettings["BarcodePortName"];
            if(sPort != "")
            {
                try
                {
                    BarcodeScanner.Instance.Open(sPort);
                }
                catch(Exception ex)
                {
                    SetInfo("打开串口失败！",true);
                }
                
            }
            //
   
        }

        private void CheckDitiBox(Layout layout)
        {
            var ditiBoxInfos = layout.DitiInfo.DitiBoxInfos;
            foreach(var ditiBoxInfo in ditiBoxInfos)
            {
                string label = ditiBoxInfo.label;
                var labware = layout.FindLabware(label);
                if (labware == null)
                {
                    SetInfo(string.Format("找不到标签号为:{0}的一次性枪头盒！", label));
                    return;
                }
                if (!labware.IsDitiBox)
                {
                    SetInfo(string.Format("标签号为:{0}的器件不是枪头盒！", label));
                    return;
                }
            }
            
        }

        private void lb_layouts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recipeParent.Children.Clear();
            Layout selectedLayout = (Layout)this.lb_layouts.SelectedItem;
            if (selectedLayout == null)
                return;
            CheckDitiBox(selectedLayout);
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
                GlobalVars.Instance.HardwareController = new Hardware.HardwareController(GlobalVars.Instance.selectedLayout);
            }  
            catch(CriticalException cEx)
            {
                SetInfo(cEx.Description, true);
                return;
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
            GlobalVars.Instance.SampleCount = smpCnt;

            //check whether labwares exist
            var selectedProtocol = _protocols[lstProtocols.SelectedIndex];
            ProtocolManager.Instance.SelectedProtocol = selectedProtocol;
            Layout selectedLayout = (Layout)this.lb_layouts.SelectedItem;
            ProtocolManager.Instance.CheckLayoutMatchProtocol(selectedLayout, selectedProtocol);
            ProtocolManager.Instance.CheckLiquidExists(PipettorElementManager.Instance.LiquidClasses, selectedProtocol);
            GlobalVars.Instance.selectedLayout = selectedLayout;
        }

        private void btnDitiSetting_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Set Diti.");

        }

    }
}
