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
using WorkstationController.Core.Data;
using System.Timers;
using WorkstationController.Hardware;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for StepMonitorForm.xaml
    /// </summary>
    public partial class StepMonitorForm : BaseUserControl
    {   
      
        Worklist wkList = new Worklist();
        ObservableCollection<StepDefinitionWithProgressInfo> stepsDefWithProgressInfo;
        RunState runState = RunState.Start;
        LogForm logForm;
        Timer timer = new Timer();
        int usedSeconds = 0;
        int timeNeeded = 0;
        int actualUsedTime = 0;
        public StepMonitorForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            log.Info("StepMonitorForm");
            logForm = new LogForm();
            logForm.Visible = false;
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timeNeeded = CalculateTimeNeeded();
            InitializeComponent();
        }

        private int CalculateTimeNeeded()
        {
            if (ProtocolManager.Instance.SelectedProtocol == null)
                return 0;

            var sampleCnt_SecondsNeed = ProtocolManager.Instance.SelectedProtocol.SampleCnt_SecondsNeed;
            //GlobalVars.Instance.SampleCount
            if (sampleCnt_SecondsNeed.Count == 0)
                return 0;

            var newDict = sampleCnt_SecondsNeed.OrderBy(x => x.Key).ToDictionary(x=>x.Key,x=>x.Value);
            if (newDict.Count == 1 || GlobalVars.Instance.SampleCount <= sampleCnt_SecondsNeed.First().Key)
            {
                int sampleCnt = newDict.First().Key;
                int seconds = newDict.First().Value;
                return (int)((GlobalVars.Instance.SampleCount / (double)sampleCnt) * seconds);
            }
            if (GlobalVars.Instance.SampleCount >= newDict.Last().Key)
            {
                int sampleCnt = newDict.Last().Key;
                int seconds = newDict.Last().Value;
                return (int)((GlobalVars.Instance.SampleCount / (double)sampleCnt) * seconds);
            }

            var smallerCnt = newDict.Where(x => x.Key < GlobalVars.Instance.SampleCount).Select(x => x.Key).Max();
            var biggerCnt =  newDict.Where(x => x.Key > GlobalVars.Instance.SampleCount).Select(x => x.Key).Min();

            double secondsPerSample = (newDict[biggerCnt] - newDict[smallerCnt]) / (biggerCnt * 1.0 - smallerCnt);
            return (int)((GlobalVars.Instance.SampleCount - smallerCnt) * secondsPerSample) + newDict[smallerCnt];
        }

 
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TotalUsed = Time
            this.Dispatcher.Invoke(() =>
            {
                txtTimeUsed.Text = TimeSpan.FromSeconds(usedSeconds).ToString();
                usedSeconds++;
                if (runState == RunState.Pause)
                    return;
                actualUsedTime++;
                if (timeNeeded != 0)
                {
                    double remainingTime = Math.Max(0, (timeNeeded - actualUsedTime));
                    txtRemainingTime.Text = remainingTime.ToString();
                }
            });
        }


        protected override void Initialize()
        {
            base.Initialize();
            InitStepsInfo();
        }

        private void InitStepsInfo()
        {
            var stepsDef = ProtocolManager.Instance.SelectedProtocol.StepsDefinition;
            stepsDefWithProgressInfo = new ObservableCollection<StepDefinitionWithProgressInfo>();
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

        private void ChangeBackGroudColor(int nStep, bool isStart)
        {
            log.InfoFormat("current step:{0}, is start:{1}", nStep, isStart);
            foreach (var thisStepWithProgressInfo in stepsDefWithProgressInfo)
            {
                if (isStart)
                {
                    thisStepWithProgressInfo.IsWorking = thisStepWithProgressInfo.LineNumber == nStep;
                    thisStepWithProgressInfo.IsFinished = thisStepWithProgressInfo.LineNumber < nStep;
                }
                else
                {
                    thisStepWithProgressInfo.IsWorking = false;
                    thisStepWithProgressInfo.IsFinished = thisStepWithProgressInfo.LineNumber <= nStep;
                }
            }
        }
        

       
        private async Task RunProtocol()
        {
            wkList = new Worklist();
            wkList.OnCriticalErrorHappened += wkList_OnCriticalErrorHappened;
            wkList.OnStepChanged += wkList_OnStepChanged;
            wkList.OnCommandInfo += wkList_OnCommandInfo;
            await Task.Run(() =>
            {
                wkList.Execute(GlobalVars.Instance.selectedLayout);
            });
        }

        private void wkList_OnCommandInfo(object sender, ITrackInfo e)
        {
           string  s ="";
           GlobalVars.Instance.TrackInfos.Add(e);
           this.Dispatcher.Invoke(() =>
            {
                if (e is PipettingTrackInfo)
                {
                    s = ((PipettingTrackInfo)e).Stringfy();
                    log.Info(s);
                    logForm.AddLog(s);
                }
            });
         
          
        }

        #region event handler
        void wkList_OnStepChanged(int currentStep, bool isStart)
        {
            this.Dispatcher.Invoke(() => {
                ChangeBackGroudColor(currentStep, isStart);
            });
           
        }

        void wkList_OnCriticalErrorHappened(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //btnStop.IsEnabled = false;
                SetErrorInfo(e);
            });
        }
        #endregion


        void SetErrorInfo(string info)
        {
            txtInfo.Text = info;
            txtInfo.Foreground = Brushes.Red;
        }

        void SetInfo(string info)
        {
            txtInfo.Text = info;
            txtInfo.Foreground = Brushes.Black;
        }

        private void btnRunPause_Click(object sender, RoutedEventArgs e)
        {
            RunPause();
        }

        protected override void onStageChanged(object sender, EventArgs e)
        {
            base.onStageChanged(sender, e);
            if (GlobalVars.Instance.FarthestStage == Stage.StepMonitor)
            {
                if (BarcodeScanner.Instance.IsOpen)
                {
                    BarcodeScanner.Instance.Close();
                }
            }
        }

        private async Task<int> RunPause()
        {
            
            if(runState == RunState.Start)
            {
                try
                {
                    GlobalVars.Instance.TrackInfos = new List<ITrackInfo>();
                    usedSeconds = 0;
                    actualUsedTime = 0;
                    timer.Start();
                    ChangeRunState(RunState.Pause);
                    //btnStop.IsEnabled = true;
                    await RunProtocol();
                }
                catch(Exception ex)
                {
                    SetErrorInfo(ex.Message);
                    ChangeRunState(RunState.Start);
                }
            }
            else if(runState == RunState.Pause)
            {
                ChangeRunState(RunState.Resume);
                wkList.PauseResume();
            }
            else
            {
                ChangeRunState(RunState.Pause);
                wkList.PauseResume();
            }
            //btnStop.IsEnabled = false; 
            return 0;
        }

        private void ChangeRunState(RunState nextState)
        {
            if (nextState == RunState.Pause)
            {
                btnStartPause.Content = parentGrid.FindResource("PauseIcon");
                runPause.Text = "暂停";
            }
            else
            {
                runPause.Text = "恢复";
                btnStartPause.Content = parentGrid.FindResource("StartIcon");
            }
            runState = nextState;
        }
     
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            SetInfo("终止运行！");
            wkList.Stop();
            ChangeRunState(RunState.Start);
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            SetInfo(""); //clear errors;
            wkList.HardwareController.Liha.Init();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            logForm.Visible = !logForm.Visible;            
        }
    }

    enum RunState
    {
        Start,
        Pause,
        Resume
    };
}
