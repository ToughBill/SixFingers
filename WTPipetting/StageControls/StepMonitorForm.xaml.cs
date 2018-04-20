﻿using System;
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
      
        Worklist wkList = new Worklist();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ObservableCollection<StepDefinitionWithProgressInfo> stepsDefWithProgressInfo;
        RunState runState = RunState.Start;
        public StepMonitorForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
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
            this.IsEnabled = false;
            await Task.Run(() =>
            {
                wkList.Execute(GlobalVars.Instance.selectedLayout);
            });
            this.IsEnabled = true;
           
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
                SetErrorInfo(e);
            });
        }
        #endregion


        void SetErrorInfo(string info)
        {
            txtInfo.Text = info;
            txtInfo.Foreground = Brushes.Red;
        }

        void UpdateInfo(string info)
        {
            txtInfo.Text = info;
            txtInfo.Foreground = Brushes.Black;
        }

        private void btnRunPause_Click(object sender, RoutedEventArgs e)
        {
            if(runState == RunState.Start)
            {
                RunProtocol();
                ChangeRunState(RunState.Pause);
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
            wkList.Stop();
        }

        
    }

    enum RunState
    {
        Start,
        Pause,
        Resume
    };
}
