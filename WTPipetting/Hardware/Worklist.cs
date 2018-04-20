using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTPipetting.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Core.Data;
using WTPipetting.Utility;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace WTPipetting.Hardware
{

    class Worklist
    {
        #region scripts
        Layout layout;
        //public System.Windows.Controls.TextBox textinfo;

      
        HardwareController hardwareController;
        List<PipettingInfo> pipettingInfos;
        bool bStop = false;
        bool bPause = false;
    

        public delegate void OnStepChangedDelegate(int currentStep, bool isStart);
        public event OnStepChangedDelegate OnStepChanged;

        public event EventHandler<string> OnCriticalErrorHappened;

      
        void NotifyStepStarted(int currentStep)
        {
            if (OnStepChanged != null)
                OnStepChanged(currentStep, true);
        }

        void NotifyStepFinished(int currentStep)
        {
            if (OnStepChanged != null)
                OnStepChanged(currentStep, false);
        }

        private void RunImpl()
        {
            
            int currentStep = 1;

            foreach (var pipettingInfo in pipettingInfos)
            {
                if (pipettingInfo.srcLabware == PipettingInfo.StepStart)
                {
                    NotifyStepStarted(currentStep);
                    continue;
                }
                if (pipettingInfo.srcLabware == PipettingInfo.StepFinish)
                {
                    NotifyStepFinished(currentStep);
                    currentStep++;
                    continue;
                }
                if (NeedPauseOrStop())
                    break;
                hardwareController.Liha.GetTip(new List<int>() { 1 });
                if (NeedPauseOrStop())
                    break;
                hardwareController.Liha.Aspirate(pipettingInfo.srcLabware, new List<int>() { pipettingInfo.srcWellID }, new List<double>() { pipettingInfo.volume }, pipettingInfo.liquidClass);
                if (NeedPauseOrStop())
                    break;
                hardwareController.Liha.Dispense(pipettingInfo.dstLabware, new List<int>() { pipettingInfo.dstWellID }, new List<double>() { pipettingInfo.volume }, pipettingInfo.liquidClass);
                if (NeedPauseOrStop())
                    break;
                hardwareController.Liha.DropTip();
            }
           
        }

        public void Run()
        {
            try
            {
               RunImpl();
            }
            catch(CriticalException ex)
            {
                if(OnCriticalErrorHappened != null)
                {
                    OnCriticalErrorHappened(this, ex.Message);
                    bStop = true;
                }
            }
        }

       
        public bool NeedPauseOrStop()
        {
            while( bPause )
                Thread.Sleep(500);
            return bStop;
        }
        public void PauseResume()
        {
            bPause = !bPause;
        }
        

        public void Stop()
        {
            bStop = true;
        }
        public void Execute(Layout layout)
        {
            this.layout = layout;
            this.hardwareController = new HardwareController(layout);
            hardwareController.Init();
            pipettingInfos = GenerateScripts();
            Run();
        }

      


        private List<PipettingInfo> GenerateScripts()
        {
            List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            int stepNo = 1;
            foreach(var stepDef in ProtocolManager.Instance.SelectedProtocol.StepsDefinition)
            {
                pipettingInfos.Add(new PipettingInfo(PipettingInfo.StepStart,0,0,"",0,""));
                pipettingInfos.AddRange(GenerateScriptsThisStep(stepDef, stepNo++));
                pipettingInfos.Add(new PipettingInfo(PipettingInfo.StepFinish, 0, 0, "", 0, ""));
            }
            
            return pipettingInfos;
        }


        private List<PipettingInfo> GenerateScriptsThisStep(StepDefinition stepDef, int stepNo)
        {

            if (stepDef.Volume == 0)
                return new List<PipettingInfo>();

            List<PipettingInfo> pipettingInfos = GetPipettingInfos(stepDef);
            return pipettingInfos;

        }

        private List<PipettingInfo> GetPipettingInfos(StepDefinition stepDef)
        {
            
            int smpCnt = GlobalVars.Instance.SampleCount;
            List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            bool isReagent = (stepDef.SourceLabware.ToLower().Contains("reagent"));
            bool isDestReagent = (stepDef.DestLabware.ToLower().Contains("reagent"));
            for (int i = 0; i < smpCnt; i++ )
            {
                int srcWell = i + 1;
                string srcLabware = stepDef.SourceLabware;
                string destLabware = stepDef.DestLabware;
                if(!isReagent)
                {
                    CalculateLabwareAndWellPosition(ref srcLabware, ref srcWell);
                }
                if (!isDestReagent)
                {
                    CalculateLabwareAndWellPosition(ref destLabware, ref srcWell);
                }
                PipettingInfo pipettingInfo = new PipettingInfo(stepDef.SourceLabware, i + 1, stepDef.Volume, stepDef.DestLabware, i + 1, stepDef.LiquidClass);
                pipettingInfos.Add(pipettingInfo);
            }
            return pipettingInfos;
        }

        private void CalculateLabwareAndWellPosition(ref string srcLabware, ref int srcWell)
        {

            var labware = layout.FindLabware(srcLabware);
            labware.CalculatePositionInLayout();
        }

       

        private List<PipettingInfo> GetPipettingInfosBatch(List<int> sourceWellIDs,
            List<int> dstWellIDs, 
            StepDefinition stepDef,
            string assayName = "")
        {
            List<string> strs = new List<string>();
            double tipType = 1000;//int.Parse(stepDef.TipType);
            double maxVolumePerTip = tipType * 0.9;
            double volumeThisStep = stepDef.Volume;
            int nTotalTimes = (int)Math.Ceiling(volumeThisStep / maxVolumePerTip);
            double finishedVolume = 0;

            string srcLabware = assayName == string.Empty ? stepDef.SourceLabware : assayName + stepDef.SourceLabware.Last();
            List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            for (int i = 0; i < nTotalTimes; i++)
            {

                double remainingVolume = volumeThisStep - finishedVolume;
                double volumeThisTime = Math.Min(remainingVolume, maxVolumePerTip);
                if (remainingVolume > maxVolumePerTip && remainingVolume - maxVolumePerTip <= tipType * 0.2)
                {
                    volumeThisTime = (int)(remainingVolume / 2.0);
                }
                finishedVolume += volumeThisTime;
                for (int j = 0; j < sourceWellIDs.Count; j++)
                {
                    pipettingInfos.Add(new PipettingInfo(srcLabware,
                        sourceWellIDs[j],
                        volumeThisTime,
                        stepDef.DestLabware,
                        dstWellIDs[j],"water"));
                }
            }
            return pipettingInfos;
        }

        private List<int> GetWellIDs(int firstWellID, int cnt, string wellConstrain)
        {
            List<int> wellIDs = new List<int>();
            int maxAllowedWellCnt = 96;
            if (IsFixedPostion(wellConstrain))
            {
                string[] strs = wellConstrain.Split('-');
                int start = int.Parse(strs[0]);
                int end = int.Parse(strs[1]);
                maxAllowedWellCnt = end - start + 1;
            }
            for (int i = 0; i < cnt; i++)
            {
                int tmpID = firstWellID + i;
                while (tmpID > maxAllowedWellCnt)
                    tmpID -= maxAllowedWellCnt;
                wellIDs.Add(tmpID);
            }
            return wellIDs;
        }



        private int GetConstrainTipCnt(string tipConstrain)
        {
            string[] strs = tipConstrain.Split('-');
            int start = int.Parse(strs[0]);
            int end = int.Parse(strs[1]);
            return end - start + 1;
        }

        private bool IsFixedPostion(string pipettingPosition)
        {
            return pipettingPosition != "*";
        }


        #endregion

      
    }


    class PipettingInfo
    {
        public string srcLabware;
        public int srcWellID;
        public double volume;
        public string dstLabware;
        public int dstWellID;
        public string liquidClass;
        public const string StepFinish = "StepFinished";
        public const string StepStart = "StepStarted";
        public PipettingInfo(string srcLabware, int srcWellID, double volume, string dstLabware, int dstWellID, string liquidClass)
        {
            this.srcLabware = srcLabware;
            this.srcWellID = srcWellID;
            this.volume = volume;
            this.dstLabware = dstLabware;
            this.dstWellID = dstWellID;
            this.liquidClass = liquidClass;
        }
    }
}
