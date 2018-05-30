using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    public class Liha : ArmBase,ILiha
    {
        XYZ xyz;
        int xMax = 650;
        int yMax = 300;
        int zMax = 200;
        TipManagement tipManagement;
        Layout layout;
        string portNum;
        const int maxSpeedV = 200;
        const int startSpeedV = 10;
        const int endSpeedV = 10;
        const int excessVolume = 10;
        public Liha(Layout layout, string portNum)
        {
            this.layout = layout;
            xyz = new XYZ(50, 0, 10);
            this.portNum = portNum;
            tipManagement = new TipManagement(layout);
            MoveController.Instance.onStepLost += Instance_onStepLost;
            Init();
        }


        public void Move2XYZ(XYZ xyz)
        {
            if(xyz.X > xMax)
            {
                throw new Exception(string.Format("x:{0}超出范围！",xyz.X));
            }
            if (xyz.Y > yMax)
            {
                throw new Exception(string.Format("y:{0}超出范围！", xyz.Y));
            }
            if (xyz.Z > zMax)
            {
                throw new Exception(string.Format("z:{0}超出范围！", xyz.Z));
            }
            log.InfoFormat("Move to: {0}_{1}_{2}", xyz.X, xyz.Y, xyz.Z);
            MoveFirstTip2AbsolutePosition((float)xyz.X, (float)xyz.Y, (float)xyz.Z);
        }

        public void MoveFirstTip2AbsolutePosition(float x, float y, float z)
        {
            Stopwatch stopWatcher = new Stopwatch();
            stopWatcher.Start();
            var res = MoveController.Instance.MoveXYZ(_eARM.左臂, (int)x, (int)y, (int)z, timeOutSeconds * 1000);
            ThrowCriticalException(res,"移动Liha");
            log.InfoFormat("used ms:{0}", stopWatcher.ElapsedMilliseconds);
        }

        public void MoveFirstTipXAbs(float x)
        {
            xyz.X = x;
            Move2XYZ(xyz);
        }

        public void MoveFirstTipYAbs(float y)
        {
            xyz.Y = y;
            Move2XYZ(xyz);
        }

        public void MoveFirstTipZAbs(float z)
        {
            xyz.Z = z;
            Move2XYZ(xyz);
        }

        public void MoveFirstTipRelativeX(float x, float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveFirstTipRelativeY(float y, float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveFirstTipRelativeZ(float z, float speed)
        {
            throw new NotImplementedException();
        }

        public void SetTipsDistance(float distance)
        {
            throw new NotImplementedException();
        }

        public void GetTip(List<int> tipIDs, out DitiTrackInfo trackInfo)
        {
            string sCommandDesc = string.Format("Get tip from ditibox:{0} remain:{1}", 
                tipManagement.CurrentLabware, 
                tipManagement.CurrentDitiID);
            log.Info(sCommandDesc);
            if (tipIDs.Count != 1)
                throw new Exception("只支持单针！");
            var tuple = Move2NextTipPosition();
            var ditiBox = tuple.Item1;
            //zDistanceFetchTip = ditiBox.ZValues.ZMax - ditiBox.ZValues.ZStart;
            //get tip
            while(true)
            {
                bool bok = TryGetTip(ditiBox.ZValues.ZMax);
                trackInfo = new DitiTrackInfo(tipManagement.CurrentLabware.Label, tipManagement.CurrentDitiID, bok);
                if (bok)
                    break;
                TipNotFetched tipNotFetched = new TipNotFetched();
                tipNotFetched.ShowDialog();
                if (tipNotFetched.UserSelection == NextActionOfNoTip.abort)
                    throw new CriticalException("取不到枪头，放弃运行程序！");
                else if (tipNotFetched.UserSelection == NextActionOfNoTip.retryNextPosition)
                    tuple = Move2NextTipPosition();
                else
                {
                    Move2SearchTipPosition(tuple);
                }
            }
        }

        private void Move2SearchTipPosition(Tuple<Labware, System.Windows.Point> tuple)
        {
            var ditibox = tuple.Item1;
            var position = tuple.Item2;
            xyz.X = position.X;
            xyz.Y = position.Y;
            xyz.Z = ditibox.ZValues.ZTravel;
            Move2XYZ(xyz);
        }

        private Tuple<Labware,System.Windows.Point> Move2NextTipPosition()
        {
            KeyValuePair<LabwareTrait, List<int>> ditiPair = new KeyValuePair<LabwareTrait, List<int>>();
            bool needRetry = false;
            try
            {
                ditiPair = tipManagement.GetTip(1).First();
            }
            catch (NoTipException notipException)
            {
                MessageBox.Show("请更换枪头！", "枪头已用完", MessageBoxButtons.OK);
                //here we replace all ditis
                tipManagement.ReplaceTips();
                needRetry = true;
            }

            if (needRetry)
            {
                ditiPair = tipManagement.GetTip(1).First();
            }

            var labware = layout.FindLabware(ditiPair.Key.Label);
            var position = labware.GetAbsPosition(ditiPair.Value.First());
            xyz.X = position.X;
            xyz.Y = position.Y;
            xyz.Z = labware.ZValues.ZStart;
            Move2XYZ(xyz);
            return Tuple.Create(labware,position);
        }

        private bool TryGetTip(double zMax)
        {
            MoveController.Instance.Move2Z(_eARM.左臂, zMax);
            return MoveController.Instance.IsTipMounted;
        }

        public void DropTip(out DitiTrackInfo ditiTrackInfo)
        {
            string sCommandDesc = "Drop tip";
            log.Info(sCommandDesc);
            var position = layout.GetWastePosition();
            xyz.X = position.X;
            xyz.Y = position.Y;
            Move2XYZ(xyz);
            while (true)
            {
                var res = MoveController.Instance.DropDiti();
                bool bok = res == e_RSPErrorCode.RSP_ERROR_NONE;
                ditiTrackInfo = new DitiTrackInfo(Labware.WasteLabel,1, bok, false);
                if(bok)
                {
                    bok = !MoveController.Instance.IsTipMounted;
                }

                if (bok)
                    break;

                DitiNotDropped ditiNotDroppedForm = new DitiNotDropped();
                ditiNotDroppedForm.ShowDialog();
                var userSelection = ditiNotDroppedForm.UserSelection;
                switch(userSelection)
                {
                    case DitiNotDroppedAction.abort:
                        throw new CriticalException("无法丢弃枪头，放弃运行程序！");
                    default:
                        break;
                }
            }
            log.Info("Drop tip finished");
        }

        public void Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, out PipettingResult pipettingResult, string barcode = "")
        {
            pipettingResult = PipettingResult.ok;
            int wellID = wellIDs.First();
            double volume = volumes.First();
            double leadingAirGap = liquidClass.AspirationSinglePipetting.LeadingAirgap;
            string sCommandDesc = string.Format("Aspirate from:{0} at:{1} volume:{2},{3}", labwareLabel, wellID, volume, liquidClass.SaveName);
            log.InfoFormat(sCommandDesc);
            Move2Position(labwareLabel, wellID);
            var labware = layout.FindLabware(labwareLabel);
            Move2Position(labwareLabel, wellID, "ZStart");
            //aspirate air gap
            var res = MoveController.Instance.Aspirate(leadingAirGap, maxSpeedV, startSpeedV, endSpeedV);
           

            ThrowCriticalException(res,"吸液");


            //检测不到或液体不够，循环询问，
            while (true)
            {
                bool bok = TryDetectLiquid(labware);
                bool hasEnoughLiquid = IsEnoughLiquid(labware, volume,liquidClass.AspirationSinglePipetting.SubMergeMM);
                if (!hasEnoughLiquid)
                    bok = false;
                if (bok)
                    break;
                else
                {
                    string title = !hasEnoughLiquid ? "液体不足" : "";
                    LiquidNotDetected liquidNotDetectForm = new LiquidNotDetected(title);
                    liquidNotDetectForm.ShowDialog();
                    var userSelection = liquidNotDetectForm.UserSelection;
                    
                    
                    switch(userSelection)
                    {
                        case NextActionOfNoLiquid.abort:
                            pipettingResult = PipettingResult.abort;
                            throw new CriticalException("无法检测到液体，放弃运行程序！");
                        case NextActionOfNoLiquid.aspirateAir:
                            pipettingResult = PipettingResult.air;
                            Move2Position(labwareLabel, wellID);
                            MoveController.Instance.Aspirate(volumes.First(), maxSpeedV,startSpeedV, endSpeedV);
                            break;
                        case NextActionOfNoLiquid.gotoZMax:
                            Move2Position(labwareLabel, wellID, "ZMax");
                            pipettingResult = PipettingResult.zmax;
                            MoveController.Instance.Aspirate(volumes.First(), maxSpeedV,startSpeedV, endSpeedV);
                            break;
                        case NextActionOfNoLiquid.retry:
                            break;
                        case NextActionOfNoLiquid.skip:
                            pipettingResult = PipettingResult.nothing;
                            throw new SkipException();
                    }
                }
            }

            //tracking 吸液
            DoTracking(labware,volume,liquidClass);
            
            res = MoveController.Instance.Aspirate(volume + excessVolume, liquidClass.AspirationSinglePipetting.AspirationSpeed, startSpeedV, endSpeedV);
            if(res == e_RSPErrorCode.RSP_ERROR_NONE)
            {
                pipettingResult = PipettingResult.ok;
            }
            else if (res == e_RSPErrorCode.凝块)
            {
                ProcessClot(labware, wellID, volume, liquidClass, out pipettingResult, barcode);
            }
            else if(res == e_RSPErrorCode.泡沫)
            {
                //currently ignore, just mark the result
                pipettingResult = PipettingResult.bubble;
            }
            else
            {
                ThrowCriticalException(res, "吸液");
                res = MoveController.Instance.Dispense(excessVolume, liquidClass.AspirationSinglePipetting.AspirationSpeed, startSpeedV, endSpeedV);
                ThrowCriticalException(res, "喷液");
                //到zStart吸 trailing airGap
                Move2Position(labwareLabel, wellID, "ZStart");
                double trailingAirGap = liquidClass.AspirationSinglePipetting.TrailingAirgap;
                MoveController.Instance.Aspirate(trailingAirGap, liquidClass.AspirationSinglePipetting.AspirationSpeed, startSpeedV, endSpeedV);
                //delay
                int delayMS = liquidClass.AspirationSinglePipetting.Delay;
                Thread.Sleep(delayMS);
            }
          

            //Move 2 ZTravel
            Move2Position(labwareLabel, wellID);

        }

        //–  Dispense back into vessel and then pipette nothing 
        //–  Ignore clot error and continue 
        //–  Discard the DITI and pipette nothing 

        private void ProcessClot(Labware labware, int wellID, double volume, LiquidClass liquidClass, out PipettingResult pipettingResult, string barcode = "")
        {
            string labwareLabel = labware.Label;
            ClotDetectedForm clotForm = new ClotDetectedForm();
            clotForm.ShowDialog();
            e_RSPErrorCode res = e_RSPErrorCode.RSP_ERROR_NONE;
            var userSelection = clotForm.UserSelection;
            pipettingResult = PipettingResult.ok;
            DitiTrackInfo trackInfo = new DitiTrackInfo(Labware.WasteLabel,1, true, false);
            switch (userSelection)
            {
                case ClotDetectedAction.dispenseBackThenDropDiti:
                    pipettingResult = PipettingResult.clotDispenseBack;
                    res = MoveController.Instance.Move2Z(_eARM.左臂, labware.ZValues.ZDispense);
                    ThrowCriticalException(res, "遇到凝块，移动到ZDispense");
                    res = MoveController.Instance.Dispense(volume + excessVolume, liquidClass.AspirationSinglePipetting.AspirationSpeed, startSpeedV, endSpeedV);
                    ThrowCriticalException(res, "遇到凝块，打回容器");
                    DropTip(out trackInfo);
                    break;
                case ClotDetectedAction.dropDiti:
                    pipettingResult = PipettingResult.clotDropDiti;
                    DropTip(out trackInfo);
                    break;
                case ClotDetectedAction.ignore:
                    pipettingResult = PipettingResult.clotIgnore;
                    break;

            }
        }

        private void DoTracking(Labware labware, double volume, LiquidClass liquidClass)
        {
            double crossSectionArea = labware.WellsInfo.WellRadius * labware.WellsInfo.WellRadius * Math.PI;
            double distance2Go = volume / crossSectionArea;
            double seconds = volume / liquidClass.AspirationSinglePipetting.AspirationSpeed;
            double goDownSpeed = distance2Go / seconds;
            MoveController.Instance.MoveZAtSpeed(_eARM.左臂, distance2Go,goDownSpeed);
        }

        private void ThrowCriticalException(e_RSPErrorCode res,string actionDesc = "")
        {
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
            {
                string addtionalInfo = actionDesc == "" ? "" : string.Format("在{0}中发生错误：", actionDesc);
                throw new CriticalException(addtionalInfo + res.ToString());
            }
          
        }

        private bool IsEnoughLiquid(Labware labware,double volume,int subMergeMM)
        {
            double x, y, z;
            x = y = z = 0;
            MoveController.Instance.GetCurrentPosition(_eARM.左臂, ref x, ref y, ref z);
            if (x == -1 || y == -1 || z == -1)
                throw new CriticalException("无法获取到位置！");
            
            double crossSectionArea = labware.WellsInfo.WellRadius * labware.WellsInfo.WellRadius * Math.PI;
            double zMax = labware.ZValues.ZMax; //mm
            return crossSectionArea * (zMax - z - subMergeMM) > volume;
        }

        private bool TryDetectLiquid(Labware labware)
        {
            const int mmPerSecond = 50;
            var res = MoveController.Instance.DetectLiquid(labware.ZValues.ZStart, labware.ZValues.ZMax, mmPerSecond);
            return res == e_RSPErrorCode.RSP_ERROR_NONE;
        }


        private void Move2Position(string labwareLabel, int wellID, string zDescription = "ZTravel")
        {
            var labware = layout.FindLabware(labwareLabel);
            var position = labware.GetAbsPosition(wellID);
            xyz.X = position.X;
            xyz.Y = position.Y;
            switch(zDescription)
            {
                case "ZTravel":
                    xyz.Z = labware.ZValues.ZTravel;
                    break;
                case "ZMax":
                    xyz.Z = labware.ZValues.ZMax;
                    break;
                case "ZStart" :
                    xyz.Z = labware.ZValues.ZStart;
                    break;
                case "ZDispense":
                    xyz.Z = labware.ZValues.ZDispense;
                    break;
            }
            Move2XYZ(xyz);
        }

        public void Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, out PipettingResult pipettingResult, string barcode = "")
        {
            int wellID = wellIDs.First();
            double volume = Math.Round(volumes.First(), 1);
            string sWellID = wellID.ToString();
            string sCommandDesc = string.Format("Dispense to:{0} at:{1} volume:{2},{3}", labwareLabel, wellID, volume, liquidClass);
            log.InfoFormat(sCommandDesc);

            //air gap
            int airGap = liquidClass.AspirationSinglePipetting.TrailingAirgap + liquidClass.AspirationSinglePipetting.LeadingAirgap;
            volume += airGap;

            Move2Position(labwareLabel, wellID);
            Move2Position(labwareLabel, wellID, "ZDispense");
            var res = MoveController.Instance.Dispense(volume, maxSpeedV, startSpeedV, endSpeedV);
            
            pipettingResult = res == e_RSPErrorCode.RSP_ERROR_NONE ? PipettingResult.ok : PipettingResult.abort;
            PipettingTrackInfo pipettingTrackInfo = new PipettingTrackInfo(labwareLabel, sWellID, volume, pipettingResult, barcode,false);
            ThrowCriticalException(res,"喷液");
            Move2Position(labwareLabel, wellID);
        }

        public void Init()
        {
            MoveController.Instance.Init(portNum);
            var res = MoveController.Instance.MoveHome(_eARM.两个,MoveController.defaultTimeOut);
            ThrowCriticalException(res, "归零");
            DitiTrackInfo ditiTrackInfo;
            DropTip(out ditiTrackInfo);
            res = MoveController.Instance.InitCarvo();
            ThrowCriticalException(res,"初始化气泵");

        }

        void Instance_onStepLost(object sender, string e)
        {
            if (onCriticalErrorHappened != null)
                onCriticalErrorHappened(this, e);
        }

       

        public bool IsTipMounted
        {
            get { return MoveController.Instance.IsTipMounted; }
        }


        public event EventHandler<string> onCriticalErrorHappened;
    }
}
