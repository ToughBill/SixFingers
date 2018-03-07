using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTPipetting.Data;
using WTPipetting.Utility;

namespace SaintX.Utility
{
    class worklist
    {
        #region scripts
        public void GenerateScripts()
        {
            var stepsDef = GetStepsDef();
       
        }

        private List<StepDefinition> GetStepsDef()
        {
            throw new NotImplementedException();
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
            return new List<PipettingInfo>();
            //Dictionary<string, List<int>> assay_wellIDs = new Dictionary<string,List<int>>();
            //for(int i = 0; i < smpCnt; i++)
            //{
            //    string assay = GlobalVars.Instance.SampleLayoutSettings[i].ColorfulAssay.Name;
            //    if (assay_wellIDs.ContainsKey(assay))
            //        assay_wellIDs[assay].Add(i+1);
            //    else
            //        assay_wellIDs.Add(assay, new List<int>() { i+1 });
            //}

            //if(stepDef.SourceLabware.ToLower().Contains("reagent"))
            //{
            //    List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            //    for (int i = 0; i < assay_wellIDs.Keys.Count; i++)
            //    {
            //        string assayName = assay_wellIDs.Keys.ElementAt(i);
            //        pipettingInfos.AddRange(GetPipettingInfosCertainAssay(stepDef, assayName, assay_wellIDs[assayName]));
            //    }
            //    pipettingInfos = pipettingInfos.OrderBy(x => x.srcWellID).ToList();
            //    return pipettingInfos;
            //}
            //else
            //{
            //    double volume = double.Parse(stepDef.Volume);
            //    int tipCountLiha = int.Parse(ConfigurationManager.AppSettings["tipCount"]);
            //    int totalTimes = (smpCnt + tipCountLiha - 1) / tipCountLiha;

            //    int tipUsedTimes = 0;
            //    List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            //    for (int batchTimes = 0; batchTimes < totalTimes; batchTimes++)
            //    {
            //        int thisTimeCnt = Math.Min(smpCnt - batchTimes * tipCountLiha, tipCountLiha);
            //        int startWellID = batchTimes * tipCountLiha + 1;
            //        List<int> sourceWellIDs = GetWellIDs(startWellID, thisTimeCnt, stepDef.AspirateConstrain);
            //        List<int> dstWellIDs = GetWellIDs(startWellID, thisTimeCnt, stepDef.DispenseConstrain);
            //        pipettingInfos.AddRange(GetPipettingInfosBatch(sourceWellIDs, dstWellIDs, stepDef));
            //    }

            //    pipettingInfos = pipettingInfos.OrderBy(x => x.srcWellID).ToList();
            //    return pipettingInfos;
            //}


            //double volume = double.Parse(stepDef.Volume);
            //int tipCountLiha = int.Parse(ConfigurationManager.AppSettings["tipCount"]);
            //int totalTimes = (smpCnt + tipCountLiha - 1) / tipCountLiha;

            //int tipUsedTimes = 0;
            //List<PipettingInfo> pipettingInfos = new List<PipettingInfo>();
            //for (int batchTimes = 0; batchTimes < totalTimes; batchTimes++)
            //{
            //    int thisTimeCnt = Math.Min(smpCnt - batchTimes * tipCountLiha, tipCountLiha);
            //    int startWellID = batchTimes * tipCountLiha + 1;
            //    List<int> sourceWellIDs = GetWellIDs(startWellID, thisTimeCnt, stepDef.AspirateConstrain);
            //    List<int> dstWellIDs = GetWellIDs(startWellID, thisTimeCnt, stepDef.DispenseConstrain);
            //    pipettingInfos.AddRange(GetPipettingInfosBatch(sourceWellIDs, dstWellIDs, stepDef));
            //}

            //pipettingInfos = pipettingInfos.OrderBy(x => x.srcWellID).ToList();
            //return pipettingInfos;
          
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
                        dstWellIDs[j]));
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

        public PipettingInfo(string srcLabware, int srcWellID, double volume, string dstLabware, int dstWellID)
        {
            this.srcLabware = srcLabware;
            this.srcWellID = srcWellID;
            this.volume = volume;
            this.dstLabware = dstLabware;
            this.dstWellID = dstWellID;
        }
    }
}
