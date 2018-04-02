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

namespace WTPipetting.Hardware
{
    class Worklist
    {
        #region scripts
        Layout layout;
        
        HardwareController hardwareController;
        public void Execute(Layout layout)
        {
            this.layout = layout;
            this.hardwareController = new HardwareController(layout);
            var pipettingInfos = GenerateScripts();
         
            foreach(var pipettingInfo in pipettingInfos)
            {
               
                hardwareController.Liha.GetTip();
                //GetTip(labware_Cnt.Key, labware_Cnt.Value.First());
                var labware = layout.FindLabware(pipettingInfo.srcLabware);
                if (labware == null)
                    throw new NoLabwareException(pipettingInfo.srcLabware);
                var position = labware.GetPosition(pipettingInfo.srcWellID);
                hardwareController.Liha.Move2AbsolutePosition(position);
                hardwareController.Liha.Aspirate(pipettingInfo.volume,pipettingInfo.liquidClass);

                labware = layout.FindLabware(pipettingInfo.dstLabware);
                if (labware == null)
                    throw new NoLabwareException(pipettingInfo.dstLabware);
                position = labware.GetPosition(pipettingInfo.srcWellID);
                hardwareController.Liha.Move2AbsolutePosition(position);
                hardwareController.Liha.Dispense(pipettingInfo.volume, pipettingInfo.liquidClass);

                System.Windows.Point pt =  layout.GetDitiPosition();
                hardwareController.Liha.Move2AbsolutePosition(position);
                hardwareController.Liha.DropTip();
            }
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
            for (int i = 0; i < smpCnt; i++ )
            {
                int srcWell = 1;
                string srcLabware = stepDef.SourceLabware;
                if(!isReagent)
                {
                    CalculateLabwareAndWellPosition(ref srcLabware,ref srcWell);
                }
                PipettingInfo pipettingInfo = new PipettingInfo(stepDef.SourceLabware, 1, stepDef.Volume, stepDef.DestLabware, i + 1, stepDef.LiquidClass);
                pipettingInfos.Add(pipettingInfo);
            }
            return pipettingInfos;
        }

        private void CalculateLabwareAndWellPosition(ref string srcLabware, ref int srcWell)
        {
            
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
