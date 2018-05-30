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
using WorkstationController.Core.Utility;

namespace WTPipetting.Hardware
{

    class Worklist
    {
        #region scripts
        Layout layout;
        //public System.Windows.Controls.TextBox textinfo;

      
        HardwareController hardwareController;
        List<ILiquidHandlerCommand> liquidHandlerCommands;
        bool bStop = false;
        bool bPause = false;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnStepChangedDelegate(int currentStep, bool isStart);
        public event OnStepChangedDelegate OnStepChanged;

        public event EventHandler<string> OnCriticalErrorHappened;
        public event EventHandler<ITrackInfo> OnCommandInfo;
      
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

        public HardwareController HardwareController
        {
            get
            {
                return hardwareController;
            }
        }

        private void RunImpl()
        {
            
            int currentStep = 1;

            foreach (var machineCommand in liquidHandlerCommands)
            {
                NotifyStepStarted(currentStep);
                 
                
                if (NeedPauseOrStop())
                    break;
                bool needBreak = false;
                if(machineCommand.CommandType == CommandType.Liha)
                {
                    needBreak= RunLihaCommand(machineCommand);
                }
                else
                {
                    needBreak = RunRomaCommand(machineCommand);
                }
                if(needBreak)
                    break;
              
                log.Info("notify finished");
                NotifyStepFinished(currentStep);
                currentStep++;
            }
        }

        private bool RunRomaCommand(ILiquidHandlerCommand machineCommand)
        {
 	        RomaCommand romaCommand = machineCommand as RomaCommand;
            Labware srcLabware = layout.FindLabware(romaCommand.SrcLabware);
            Labware dstLabware = layout.FindLabware(romaCommand.DstLabware);
            //srcLabware.PlateVector.Positions
            
            bool needBreak = RunPlateVector(srcLabware.PlateVector);
            ITrackInfo romaTrackInfo = new RomaTrackInfo(srcLabware.Label,dstLabware.Label,false);
            //OnCommandInfo(this,string.Format("Move to source plate:{0}",srcLabware.Label));
            if(needBreak)
            {
                OnCommandExecuted(romaTrackInfo);
                return true;
            }
                

            needBreak = RunPlateVector(dstLabware.PlateVector);
            ((RomaTrackInfo)romaTrackInfo).AllFinished = true;
            //OnCommandInfo(this,string.Format("Move to dst plate:{0}",dstLabware.Label));
            OnCommandExecuted(romaTrackInfo );
            return needBreak;

        }

        private bool RunPlateVector(PlateVector plateVector)
        {
 	        var safeVector = plateVector.Positions.First(x=>x.ID == PlateVector.Safe);
            var endVector = plateVector.Positions.First(x=>x.ID == PlateVector.End);
            List<ROMAPosition> romaPositions = new List<ROMAPosition>();
            foreach(var position in plateVector.Positions)
            {
                if(position.ID == PlateVector.Safe || position.ID == PlateVector.End)
                    continue;
                romaPositions.Add(position);
            }
            
            foreach(var position in romaPositions)
            {
                if (NeedPauseOrStop())
                    return true;
                MoveRoma(position);
            }
            if (NeedPauseOrStop())
                return true;
            MoveRoma(safeVector);
            return false;
        }

        private void MoveRoma(ROMAPosition position)
        {
            hardwareController.Roma.Move2AbsPosition(position.X,position.Y,position.Z);
            hardwareController.Roma.MoveClipper(position.R, position.ClipDistance);
        }

        private bool RunLihaCommand(ILiquidHandlerCommand machineCommand)
        {
            
            if(hardwareController.Liha.IsTipMounted)
            {
                DitiTrackInfo trackInfo = null;
                hardwareController.Liha.DropTip(out trackInfo);
            }
            
            
            DitiTrackInfo ditiTrackInfos = null;
            hardwareController.Liha.GetTip(new List<int>() { 1 }, out ditiTrackInfos);
            LihaCommand lihaCommand = machineCommand as LihaCommand;
            OnCommandExecuted(ditiTrackInfos);
            if (NeedPauseOrStop())
                return true;
            var liquidClass = PipettorElementManager.Instance.LiquidClasses.First(x => x.SaveName == lihaCommand.liquidClass);
            
            bool needSkipDispense = false;
            try
            {
                PipettingResult pipettingResult = PipettingResult.ok;
                hardwareController.Liha.Aspirate(lihaCommand.srcLabware, new List<int>() { lihaCommand.srcWellID }, new List<double>() { lihaCommand.volume }, liquidClass, out pipettingResult);
                PipettingTrackInfo trackInfo = new PipettingTrackInfo(lihaCommand.srcLabware, LihaCommand.GetWellDesc(lihaCommand.srcWellID),lihaCommand.volume, pipettingResult, lihaCommand.barcode);
                OnCommandExecuted(trackInfo);
            }
            catch(SkipException ex)
            {
                needSkipDispense = true;
            }
            
            
            if (NeedPauseOrStop())
                return true;
            if(!needSkipDispense) //if need skip
            {
                PipettingResult pipettingResult = PipettingResult.ok;
                hardwareController.Liha.Dispense(lihaCommand.dstLabware, new List<int>() { lihaCommand.dstWellID }, new List<double>() { lihaCommand.volume }, liquidClass, out pipettingResult);
                PipettingTrackInfo trackInfo = new PipettingTrackInfo(lihaCommand.dstLabware,LihaCommand.GetWellDesc(lihaCommand.dstWellID), lihaCommand.volume, pipettingResult, lihaCommand.barcode);
                OnCommandExecuted(trackInfo);
            }
            
            if (NeedPauseOrStop())
                return true;
            DitiTrackInfo ditiTrackInfo = null;
            hardwareController.Liha.DropTip(out ditiTrackInfo);
            OnCommandExecuted(ditiTrackInfo);
            return false;
        }

        private void OnCommandExecuted(ITrackInfo trackInfo)
        {
            if (OnCommandInfo != null)
                OnCommandInfo(this, trackInfo);
        }

        public void Run()
        {
            try
            {
               this.hardwareController = GlobalVars.Instance.HardwareController;  //new HardwareController(layout);
               RunImpl();
            }
            catch(CriticalException ex)
            {
                if(OnCriticalErrorHappened != null)
                {
                    OnCriticalErrorHappened(this, ex.Description);
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
            liquidHandlerCommands = GenerateCommands();
            Run();
        }

      


        private List<ILiquidHandlerCommand> GenerateCommands()
        {
            List<ILiquidHandlerCommand> machineCommands = new List<ILiquidHandlerCommand>();
            int stepNo = 1;
            foreach(var stepDef in ProtocolManager.Instance.SelectedProtocol.StepsDefinition)
            {
                //machineCommands.AddRange(GenerateScriptsThisStep(stepDef, stepNo));
                if(stepDef.LiquidClass == StepDefinition.MovePlate)
                {
                    machineCommands.Add(GenerateMovePlateCommand(stepDef));
                }
                else
                {
                    machineCommands.AddRange(GenerateLihaCommandsThisStep(stepDef, stepNo));
                }
                stepNo++;
            }
            return machineCommands;
            //return pipettingInfos;
        }

        private ILiquidHandlerCommand GenerateMovePlateCommand(StepDefinition stepDef)
        {
 	        RomaCommand romaCommand = new RomaCommand(stepDef.SourceLabware,stepDef.DestLabware);
            return romaCommand;
        }


        private List<LihaCommand> GenerateLihaCommandsThisStep(StepDefinition stepDef, int stepNo)
        {
            if (stepDef.Volume == 0)
                return new List<LihaCommand>();
            List<LihaCommand> pipettingInfos = GetPipettingInfos(stepDef);
            return pipettingInfos;

        }

        private List<LihaCommand> GetPipettingInfos(StepDefinition stepDef)
        {
            
            int smpCnt = GlobalVars.Instance.SampleCount;
            List<LihaCommand> pipettingInfos = new List<LihaCommand>();
            for (int i = 0; i < smpCnt; i++ )
            {
                int srcWell = i + 1;
                string srcLabware = FindSourceLabware(ref srcWell,stepDef.SourceLabware);
                string destLabware = stepDef.DestLabware;
                List<int> sourceWellIDs = new List<int>(){i+1};
                List<int> dstWellIDs = sourceWellIDs;
                var lihaCommands = GetLihaCommandsThisBatch(sourceWellIDs,dstWellIDs,stepDef);
                pipettingInfos.AddRange(lihaCommands);
            }
            return pipettingInfos;
        }

        private string FindSourceLabware(ref int srcWell, string firstSrcLabware)
        {
            if (srcWell <= 16)
                return firstSrcLabware;
            string commonPart = firstSrcLabware.Substring(0, firstSrcLabware.Length - 1);
            int gridID = (srcWell - 1) / 16 + 1;
            srcWell = srcWell - (gridID - 1) * 16;
            return commonPart + gridID.ToString();
        }

       

        private List<LihaCommand> GetLihaCommandsThisBatch(List<int> sourceWellIDs,
            List<int> dstWellIDs, 
            StepDefinition stepDef)
        {
            List<string> strs = new List<string>();
            double tipType = 1000;//int.Parse(stepDef.TipType);
            double maxVolumePerTip = tipType * 0.9;
            double volumeThisStep = stepDef.Volume;
            int nTotalTimes = (int)Math.Ceiling(volumeThisStep / maxVolumePerTip);
            double finishedVolume = 0;

            List<LihaCommand> lihaCommands = new List<LihaCommand>();
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
                    string sBarcode = GlobalVars.Instance.Tube_Barcode[j];
                    lihaCommands.Add(new LihaCommand(sBarcode,stepDef.SourceLabware,
                        sourceWellIDs[j],
                        volumeThisTime,
                        stepDef.DestLabware,
                        dstWellIDs[j],"water"));
                }
            }
            return lihaCommands;
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

    public interface ILiquidHandlerCommand
    {
        CommandType CommandType { get;  }
    }

    public enum CommandType
    {
        Liha,
        Roma
    }

    public class RomaCommand:ILiquidHandlerCommand
    {
        string srcLabware;
        string dstLabware;

        public RomaCommand (string srcLabware, string dstLabware)
        {
            this.srcLabware = srcLabware;
            this.dstLabware = dstLabware;
        }
    
        
        public string SrcLabware 
        {
            get
            {
                return srcLabware;
            }
        }

        public string DstLabware
        {
            get{
                return dstLabware;
            }
        }

        public CommandType CommandType
        {
	        get { return Hardware.CommandType.Roma; }
        }
}

    public class LihaCommand:ILiquidHandlerCommand
    {
        public string barcode;
        public string srcLabware;
        public int srcWellID;
        public double volume;
        public string dstLabware;
        public int dstWellID;
        public string liquidClass;
        public LihaCommand(string barcode,string srcLabware, int srcWellID, double volume, string dstLabware, int dstWellID, string liquidClass)
        {
            this.barcode = barcode;
            this.srcLabware = srcLabware;
            this.srcWellID = srcWellID;
            this.volume = volume;
            this.dstLabware = dstLabware;
            this.dstWellID = dstWellID;
            this.liquidClass = liquidClass;
        }

        public static string GetWellDesc(int wellID)
        {
            int colIndex = (wellID - 1) / 8;
            int rowIndex = wellID - colIndex * 8 - 1;
            return string.Format("{0}{1}", (char)('A' + rowIndex), colIndex + 1);
        }


        public CommandType CommandType
        {
            get { return Hardware.CommandType.Liha; }
        }
    }
}
