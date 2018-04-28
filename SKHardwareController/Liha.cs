using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    public class Liha : ArmBase,ILiha
    {
        XYZ xyz;
        int xMax = 800;
        int yMax = 300;
        int zMax = 200;
        TipManagement tipManagement;
        Layout layout;
         
        
        public Liha(Layout layout, string portNum)
        {
            this.layout = layout;
            xyz = new XYZ(50, 0, 10);
            tipManagement = new TipManagement(layout);
            MoveController.Instance.Init(portNum);
            MoveController.Instance.MoveHome(_eARM.左臂, timeOutSeconds*2000);
            MoveController.Instance.MoveHome(_eARM.右臂, timeOutSeconds*2000);

        }

        private void Move2XYZ(XYZ xyz)
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
            MoveController.Instance.MoveXYZ(_eARM.左臂, (int)x, (int)y,(int) z, timeOutSeconds * 1000);
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

        public void GetTip(List<int> tipIDs)
        {
            log.InfoFormat("Get tip from ditibox:{0} remain:{1}", tipManagement.CurrentLabware, tipManagement.CurrentDitiID);
            if (tipIDs.Count != 1)
                throw new Exception("只支持单针！");
            var ditiPair = tipManagement.GetTip(1).First();
            var labware = layout.FindLabware(ditiPair.Key.Label);
            var position = labware.GetAbsPosition(ditiPair.Value.First());
            xyz.X = position.X;
            xyz.Y = position.Y;
            Move2XYZ(xyz);

        }

        public void DropTip()
        {
            log.Info("Drop tip");
            var position = layout.GetWastePosition();
            xyz.X = position.X;
            xyz.Y = position.Y;
            Move2XYZ(xyz);
        }

        public void Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass)
        {
            log.InfoFormat("A;{0};{1};{2},{3}", labwareLabel, wellIDs.First(),volumes.First(),liquidClass);
            Move2Position(labwareLabel, wellIDs.First());
            
        }

        private void Move2Position(string labwareLabel, int wellID)
        {
            var labware = layout.FindLabware(labwareLabel);
            var position = labware.GetAbsPosition(wellID);
            xyz.X = position.X;
            xyz.Y = position.Y;
            Move2XYZ(xyz);
        }

        public void Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass)
        {
            log.InfoFormat("D;{0};{1};{2},{3}", labwareLabel, wellIDs.First(), volumes.First(), liquidClass);
            Move2Position(labwareLabel, wellIDs.First());
        }

        public void Init()
        {
           
        }
    }
}
