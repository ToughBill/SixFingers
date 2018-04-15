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


    class MyBackThread
    {
        public delegate void OnWorkerMethodStartDelegate(string templateName, string templatePath, int curIndex);
        public event OnWorkerMethodStartDelegate OnWorkerStart;

        public delegate void OnWorkerMethodCompleteDelegate(double[,] measureResultData, int curIndex);
        public event OnWorkerMethodCompleteDelegate OnWorkerComplete;

        public delegate void OnWorkerMethodTemplateCompleteDelegate(string templateName, string templatePath);
        public event OnWorkerMethodTemplateCompleteDelegate OnTemplateComplete;

        public delegate void OnMoveToDelegate(Point position);
        public event OnMoveToDelegate OnMoveTo;

        public delegate void OnMessageDelegate(string strMessage);
        public event OnMessageDelegate OnMessage;

        //Process process = new Process();
        Thread myThread;
        bool bQuickStop = false;
        Worklist _worklist;

        public MyBackThread(Worklist worklist, bool bSingleTest)
        {
            _worklist = worklist;
            ThreadStart tStart;
            if (bSingleTest)
                tStart = new ThreadStart(this.SingleProcess);
            else
                tStart = new ThreadStart(this.WorklistProcess);
            myThread = new Thread(tStart);
        }
        public void SingleProcess()
        {

            //process = new Process();
            //process.SinglePointMeasure();
            //measureResultData = process.measureResultData;
            //Thread.Sleep(5000);
            //OnWorkerComplete(process.measureResultData, 0);
        }
        public void WorklistProcess()
        {
            _worklist.Run();
        }
        public void Start()
        {
            myThread.Start();
        }

        public void Stop()
        {
            bQuickStop = true;
            if (myThread != null)
            {
                //myThread.Join();
                myThread = null;
                //process = null;
            }
        }
        public void Message(string message)
        {
            OnMessage(message);
        }
    }
    class Worklist
    {
        #region scripts
        Layout layout;
        //public System.Windows.Controls.TextBox textinfo;

        public MyBackThread myBackThread;
        HardwareController hardwareController;
        List<PipettingInfo> pipettingInfos;
        bool bQuickStop = false;
        bool bPause = false;
        bool bRunning = false;

        public MyBackThread Init()
        {

            myBackThread = new MyBackThread(this, false);

            return myBackThread;
        }

        public void Run()
        {
            bRunning = true;

            foreach (var pipettingInfo in pipettingInfos)
            {

                hardwareController.Liha.GetTip();
                //GetTip(labware_Cnt.Key, labware_Cnt.Value.First());
                var carrier = layout.FindCarrierByLabware(pipettingInfo.srcLabware);
                var labware = layout.FindLabware(pipettingInfo.srcLabware);
                if (labware == null)
                    throw new NoLabwareException(pipettingInfo.srcLabware);
                var position = labware.GetPosition(pipettingInfo.srcWellID);
                position = layout.GetPosition(carrier, labware, pipettingInfo.srcWellID);
                if (CheckStop())
                    break;
                hardwareController.Liha.Move2AbsolutePosition(position);

                myBackThread.Message("Move " + position.X + " " + position.Y);

                if (CheckStop())
                    break;
                hardwareController.Liha.Aspirate(pipettingInfo.volume, pipettingInfo.liquidClass);

                myBackThread.Message("Aspirate " + pipettingInfo.volume + " " + pipettingInfo.liquidClass);

                carrier = layout.FindCarrierByLabware(pipettingInfo.dstLabware);
                labware = layout.FindLabware(pipettingInfo.dstLabware);
                if (labware == null)
                    throw new NoLabwareException(pipettingInfo.dstLabware);
                position = labware.GetPosition(pipettingInfo.dstWellID);
                position = layout.GetPosition(carrier, labware, pipettingInfo.dstWellID);

                if (CheckStop())
                    break;
                hardwareController.Liha.Move2AbsolutePosition(position);

                myBackThread.Message("Move " + position.X + " " + position.Y);

                if (CheckStop())
                    break;
                hardwareController.Liha.Dispense(pipettingInfo.volume, pipettingInfo.liquidClass);

                myBackThread.Message("Dispense " + pipettingInfo.volume + " " + pipettingInfo.liquidClass);

                System.Windows.Point pt = layout.GetWastePosition(); //here need to fix
                if (CheckStop())
                    break;
                hardwareController.Liha.Move2AbsolutePosition(position);

                if (CheckStop())
                    break;
                hardwareController.Liha.DropTip();
            }

            bRunning = false;
        }

        public bool IsRunning()
        {
            return bRunning;
        }
        public bool CheckStop()
        {
            while( bPause )
                Thread.Sleep(500);
            return bQuickStop;
        }
        public void Pause()
        {
            bPause = !bPause;
        }
        public void Stop()
        {
            bQuickStop = true;
        }
        public void Execute(Layout layout)
        {
            this.layout = layout;
            this.hardwareController = new HardwareController(layout);
            hardwareController.Init();
            pipettingInfos = GenerateScripts();
            myBackThread.Start();
        }

      


        private List<PipettingInfo> GenerateScripts()
        {
            List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            int stepNo = 1;
            foreach(var stepDef in ProtocolManager.Instance.SelectedProtocol.StepsDefinition)
            {
                pipettingInfos.AddRange(GenerateScriptsThisStep(stepDef, stepNo++));
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
