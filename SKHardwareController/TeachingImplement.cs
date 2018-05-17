using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    public class TeachingImplement : ITeachingController
    {
        Dictionary<ArmType, _eARM> enumMapper = new Dictionary<ArmType, _eARM>();
        XYZR startPosition;
        double clipWidth;
        double degree;
        WorkstationController.Hardware.Direction dir;
        int speedMMPerSecond;
        ArmType armType;
        public TeachingImplement()
        {
            string sPort = ConfigurationManager.AppSettings["PortName"];
            Init(sPort);
        }
        public void Init(string sPort)
        {
            enumMapper.Add(ArmType.Liha, _eARM.左臂);
            enumMapper.Add(ArmType.Roma, _eARM.右臂);
            MoveController.Instance.Init(sPort);
            MoveController.Instance.MoveHome(_eARM.两个,MoveController.defaultTimeOut);
        }

        public void Move2XYZR(ArmType armType, XYZR xyzr)
        {
            //need consider ztravel
            var err = MoveController.Instance.MoveXYZ(enumMapper[armType], xyzr.X, xyzr.Y, xyzr.Z, MoveController.defaultTimeOut);
            if (err != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(err.ToString());
        }

        public XYZR GetPosition(ArmType armType)
        {
            double x, y, z,r;
            x = y = z = r = 0;
            MoveController.Instance.GetCurrentPosition(enumMapper[armType], ref x, ref y, ref z);
            return new XYZR(x, y, z);
        }

  

        public void StopMove()
        {
            MoveController.Instance.StopMove();
            Thread.Sleep(100);
            if(armType == ArmType.Roma && IsRotateOrClip(dir))
            {
                double currentDegree,currentClipWidth;
                currentClipWidth = currentDegree = 0;
                GetClipperInfo(ref currentDegree, ref currentClipWidth);

                bool needAdditionalMove = false;
                double dstDegree, dstClipWidth;
                dstDegree = currentDegree;
                dstClipWidth = currentClipWidth;

                if(dir == WorkstationController.Hardware.Direction.RotateCCW || dir == WorkstationController.Hardware.Direction.RotateCW)
                {
                    double degreeDiff = Math.Abs(currentDegree - degree);
                    if(degreeDiff < speedMMPerSecond *0.1)
                    {
                        needAdditionalMove = true;
                        double minusOrPlus = dir == WorkstationController.Hardware.Direction.RotateCW ?  1: -1;
                        dstDegree = degree + minusOrPlus * speedMMPerSecond * 0.1;
                    }
                }
                if(dir == WorkstationController.Hardware.Direction.ClampOff || dir == WorkstationController.Hardware.Direction.ClampOn)
                {
                    double clipDiff = currentClipWidth - clipWidth;
                    if (clipDiff < speedMMPerSecond * 0.1)
                    {
                        needAdditionalMove = true;
                        double minusOrPlus = dir == WorkstationController.Hardware.Direction.ClampOn ? 1 : -1;
                        dstClipWidth = degree + minusOrPlus* speedMMPerSecond * 0.1;
                    }
                }
                if(needAdditionalMove)
                    MoveClipper(dstDegree, dstClipWidth);
                return;
            }
            var currentPosition = GetPosition(armType);
            double distanceMoved = GetDistance(startPosition, currentPosition, dir);
            if(distanceMoved < speedMMPerSecond * 0.1)
            {
                XYZR dstPosition = GetDstPosition(startPosition, dir, speedMMPerSecond * 0.1);
                Move2XYZR(armType, dstPosition);
            }
        }

        private XYZR GetDstPosition(XYZR startPosition, WorkstationController.Hardware.Direction dir, double distance)
        {
            XYZR xyzr = new XYZR(startPosition);
            switch (dir)
            {
                case WorkstationController.Hardware.Direction.Down:
                    xyzr.Y -= distance;
                    break;
                case WorkstationController.Hardware.Direction.Up:
                    xyzr.Y += distance;
                    break;
                case WorkstationController.Hardware.Direction.Left:
                    xyzr.X -= distance;
                    break;
                case WorkstationController.Hardware.Direction.Right:
                    xyzr.X += distance;
                    break;
                case WorkstationController.Hardware.Direction.ZDown:
                    xyzr.Z -= distance;
                    break;
                case WorkstationController.Hardware.Direction.ZUp:
                    xyzr.Z += distance;
                    break;
                default:
                    break;
            }
            return xyzr;
        }

        private bool IsRotateOrClip(WorkstationController.Hardware.Direction dir)
        {
            return dir == WorkstationController.Hardware.Direction.ClampOff ||
                   dir == WorkstationController.Hardware.Direction.ClampOn ||
                   dir == WorkstationController.Hardware.Direction.RotateCW ||
                   dir == WorkstationController.Hardware.Direction.RotateCCW;
        }

        private double GetDistance(XYZR startPosition, XYZR currentPosition, WorkstationController.Hardware.Direction dir)
        {
            double distance = 0;
            switch(dir)
            {
                case WorkstationController.Hardware.Direction.Down:
                case WorkstationController.Hardware.Direction.Up:
                    distance = currentPosition.Y - startPosition.Y;
                    break;
                case WorkstationController.Hardware.Direction.Left:
                case WorkstationController.Hardware.Direction.Right:
                    distance = currentPosition.X - startPosition.X;
                    break;
                case WorkstationController.Hardware.Direction.ZDown:
                case WorkstationController.Hardware.Direction.ZUp:
                    distance = currentPosition.Z - startPosition.Z;
                    break;
                default:
                    break;
            }
            return Math.Abs(distance);
        }

        public void StartMove(ArmType armType, WorkstationController.Hardware.Direction dir, int speedMMPerSecond)
        {
            startPosition = GetPosition(armType);
            this.dir = dir;
            this.armType = armType;
            this.speedMMPerSecond = speedMMPerSecond;
            if(armType == ArmType.Roma)
            {
                GetClipperInfo(ref degree, ref clipWidth);
            }
            var res = MoveController.Instance.StartMove(enumMapper[armType], (Direction)dir, speedMMPerSecond);
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(res.ToString());
        }


        public void MoveClipper(double degree, double clipWidth)
        {
            var res = MoveController.Instance.MoveClipper(degree, clipWidth);
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(res.ToString());
        }

        public void GetClipperInfo(ref double degree, ref double clipWidth)
        {
            var res = MoveController.Instance.GetClipperInfo(ref degree, ref clipWidth);
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(res.ToString());
        }
    }
}
