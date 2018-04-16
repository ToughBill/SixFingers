using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WTPipetting.Data;
using WTPipetting.Navigation;
using WTPipetting.Utility;
using WTPipetting.Hardware;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for StepMonitorForm.xaml
    /// </summary>
    public partial class StepMonitorForm : BaseUserControl
    {   
        public string textLog = "You are in runing mode";
        Worklist wkList = new Worklist();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public StepMonitorForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
            txtInfo.Text = textLog;
        }


        protected override void Initialize()
        {
            txtInfo.Text = textLog;
            base.Initialize();
            InitStepsInfo();
        }

        private void InitStepsInfo()
        {
            var stepsDef = ProtocolManager.Instance.SelectedProtocol.StepsDefinition;
            ObservableCollection<StepDefinitionWithProgressInfo> stepsDefWithProgressInfo = new ObservableCollection<StepDefinitionWithProgressInfo>();
            //timeEstimation = new TimeEstimation();
            
            stepsDefWithProgressInfo.Clear();
            foreach (var stepDef in stepsDef)
            {
                var stepDefEx = new StepDefinitionWithProgressInfo(stepDef);
                
                stepsDefWithProgressInfo.Add(stepDefEx);
            }
            //timeInfo.DataContext = timeEstimation;
            lvProtocol.ItemsSource = stepsDefWithProgressInfo;
        }



        private void OnWorkerMethodStart(string tempName, string currentTemplate, int curIndex)
        {

            //lvwArrayTemplate.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            //new Action(
            //delegate()
            //{
            //    if (CurIndex == 0)
            //    {

            //        List<string> lists = LoadStrListFromFile(currentTemplate);
            //        LoadToTemplateView(lists);
            //        txtTemplateName.Content = tempName;
            //        txtTemplatePath.Content = currentTemplate;
            //    }
            //    //CalcShowResult();
            //    lvwArrayTemplate.SelectedIndex = curIndex;
            //    //pbw.Close();
            //}
            //));
        }

        private void OnWorkerMethodComplete(double[,] measureResultData, int curIndex)
        {
            //CurIndex = curIndex;
            ////this.measureResultData = measureResultData;
            //for (int i = 0; i < 12; i++)
            //{
            //    this.measureResultData[curIndex, i] = measureResultData[curIndex, i];

            //}
            //this.measureResultData[curIndex, 0] = curIndex;//序号

            //txtInfo.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            //new Action(
            //delegate()
            //{
            //    CalcShowResult();
            //    //pbw.Close();
            //}
            //));

            // myLabel.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            //new Action(
            //delegate()
            //{
            //    myLabel.Content = message;
            //}
            //));

        }

        private void OnTemplateComplete(string tempName, string currentTemplate)
        {
        }


        public void OnMessage(string strMessage)
        {
            txtInfo.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                txtInfo.Text += strMessage + "\r\n";
            }
            ));
        }
        public void OnMoveTo(Point position)
        {
            txtInfo.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(
            delegate()
            {
                txtInfo.Text += "Move " + position.X + " " + position.Y + "\r\n";
            }
            ));
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (wkList!=null && wkList.IsRunning())
            {
                return;
            }
            wkList = new Worklist();
            MyBackThread myBackThread = wkList.Init();
            myBackThread.OnWorkerStart += new MyBackThread.OnWorkerMethodStartDelegate(OnWorkerMethodStart);
            myBackThread.OnWorkerComplete += new MyBackThread.OnWorkerMethodCompleteDelegate(OnWorkerMethodComplete);
            myBackThread.OnMoveTo += new MyBackThread.OnMoveToDelegate(OnMoveTo);
            myBackThread.OnMessage += new MyBackThread.OnMessageDelegate(OnMessage);
            myBackThread.OnTemplateComplete += new MyBackThread.OnWorkerMethodTemplateCompleteDelegate(OnTemplateComplete);

            wkList.Execute(GlobalVars.Instance.selectedLayout);
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            wkList.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            wkList.Stop();
        }
    }
}
