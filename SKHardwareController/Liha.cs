using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    public class Liha : ArmBase,ILiha
    {
        XYZR xyz;
        int xMax = 650;
        int yMax = 300;
        int zMax = 200;
        TipManagement tipManagement;
        Layout layout;
        string portNum;
        
        public Liha(Layout layout, string portNum)
        {
            this.layout = layout;
            xyz = new XYZR(50, 0, 10);
            this.portNum = portNum;
            tipManagement = new TipManagement(layout);
            Init();
        }

        

        public void Move2XYZ(XYZR xyz)
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
            var err = MoveController.Instance.MoveXYZ(_eARM.左臂, (int)x, (int)y,(int) z, timeOutSeconds * 1000);

            //if(MoveController.Instance.ErrorHappened)
            //{
            //    throw new CriticalException(err.ToString());
            //}
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

        public string GetTip(List<int> tipIDs)
        {
            string sCommandDesc = string.Format("Get tip from ditibox:{0} remain:{1}", 
                tipManagement.CurrentLabware, 
                tipManagement.CurrentDitiID);
            log.Info(sCommandDesc);
            if (tipIDs.Count != 1)
                throw new Exception("只支持单针！");
            double zDistanceFetchTip = 0; //取枪头移动距离
            var tuple = Move2NextTipPosition();
            var ditiBox = tuple.Item1;
            zDistanceFetchTip = ditiBox.ZValues.ZMax - ditiBox.ZValues.ZStart;
            //get tip
            while(true)
            {
                bool bok = TryGetTip(zDistanceFetchTip);
                if (!bok)
                {
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
                else
                    break;
            }
            return sCommandDesc;
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

        private bool TryGetTip(double zDistanceFetchTip)
        {
            MoveController.Instance.MoveZAtSpeed(_eARM.左臂, zDistanceFetchTip);
            return MoveController.Instance.IsTipMounted;
        }

        public string DropTip()
        {
            string sCommandDesc = "Drop tip";
            log.Info(sCommandDesc);
            var position = layout.GetWastePosition();
            xyz.X = position.X;
            xyz.Y = position.Y;
            Move2XYZ(xyz);
            log.Info("Drop tip finished");
            return sCommandDesc;
        }

        public string Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass)
        {
            string sCommandDesc = string.Format("Aspirate from:{0} at:{1} volume:{2},{3}", labwareLabel, wellIDs.First(), volumes.First(), liquidClass);
            log.InfoFormat(sCommandDesc);
            Move2Position(labwareLabel, wellIDs.First());
            var labware = layout.FindLabware(labwareLabel);
            Move2PositionZStart(labwareLabel, wellIDs.First());
            int mmPerSecond = 50;
            var res = MoveController.Instance.DetectLiquid(labware.ZValues.ZStart, labware.ZValues.ZMax, mmPerSecond);
            if(res == e_RSPErrorCode.RSP_ERROR_NONE)
            {

            }
            //not detected
            //not enough liquid
            //air,goto zmax
            //jump the sample
            return sCommandDesc;
        }

        private void Move2PositionZStart(string labwareLabel, int wellID)
        {
            var labware = layout.FindLabware(labwareLabel);
            var position = labware.GetAbsPosition(wellID);
            xyz.X = position.X;
            xyz.Y = position.Y;
            xyz.Z = labware.ZValues.ZStart;
            Move2XYZ(xyz);
        }

        private void Move2Position(string labwareLabel, int wellID)
        {
            var labware = layout.FindLabware(labwareLabel);
            var position = labware.GetAbsPosition(wellID);
            xyz.X = position.X;
            xyz.Y = position.Y;
            xyz.Z = labware.ZValues.ZTravel;
            Move2XYZ(xyz);
        }

        public string Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass)
        {
            string sCommandDesc = string.Format("Dispense to:{0} at:{1} volume:{2},{3}", labwareLabel, wellIDs.First(), volumes.First(), liquidClass);
            log.InfoFormat(sCommandDesc);
            Move2Position(labwareLabel, wellIDs.First());
            return sCommandDesc;
        }

        public void Init()
        {
            MoveController.Instance.Init(portNum);
            MoveController.Instance.MoveHome(_eARM.两个,MoveController.defaultTimeOut);
        }


    }
}
