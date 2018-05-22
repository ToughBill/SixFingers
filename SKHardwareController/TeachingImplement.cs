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
            
        }
        public void Init()
        {
            string sPort = ConfigurationManager.AppSettings["PortName"];
            enumMapper.Add(ArmType.Liha, _eARM.左臂);
            enumMapper.Add(ArmType.Roma, _eARM.右臂);
            MoveController.Instance.Init(sPort);
            var res = MoveController.Instance.MoveHome(_eARM.两个,MoveController.defaultTimeOut);
            ThrowIfErrorHappened(res);
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
            Axis axis = ConvertDir2Axis(dir);
            dir = Direction.None;
            MoveController.Instance.StopMove(enumMapper[armType], axis);
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

        private Axis ConvertDir2Axis(Direction dir)
        {
            switch(dir)
            {
                case Direction.Left:
                case Direction.Right:
                    return Axis.X;
                case Direction.Up:
                case Direction.Down:
                    return Axis.Y;
                case Direction.ZUp:
                case Direction.ZDown:
                    return Axis.Z;
                case Direction.RotateCCW:
                case Direction.RotateCW:
                    return Axis.R;
                case Direction.ClampOn:
                case Direction.ClampOff:
                    return Axis.Clipper;
                default:
                    throw new Exception("不知道之前的移动方向，无法停止");
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
                    xyzr.Z += distance;
                    break;
                case WorkstationController.Hardware.Direction.ZUp:
                    xyzr.Z -= distance;
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

            var eArmType = enumMapper[armType];
            //var res = MoveController.Instance.StartMove(enumMapper[armType], (Direction)dir, speedMMPerSecond);
            e_RSPErrorCode res = e_RSPErrorCode.RSP_ERROR_NONE;
            e_CanMotorID motorID = e_CanMotorID.CanMotorID_Max;
            switch(dir)
            {
                case Direction.Left:
                case Direction.Right:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_x : e_CanMotorID.CanMotorID_Right_x;
                    res = MoveController.Instance.SetSpeed(eArmType, motorID, speedMMPerSecond);
                    ThrowIfErrorHappened(res);
                    res = MoveController.Instance.Move2X(eArmType, dir == Direction.Left ?  0 : 700);
                    break;
                case Direction.Up:
                case Direction.Down:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_y : e_CanMotorID.CanMotorID_Right_y;
                    MoveController.Instance.SetSpeed(eArmType, motorID, speedMMPerSecond);
                    res = MoveController.Instance.Move2Y(eArmType, dir == Direction.Down ? 0 : 400);
                    break;
                case Direction.ZDown:
                case Direction.ZUp:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_z : e_CanMotorID.CanMotorID_Right_z;
                    MoveController.Instance.SetSpeed(eArmType, motorID, speedMMPerSecond);
                    res = MoveController.Instance.Move2Z(eArmType, dir == Direction.ZUp ? 0 : 300);
                    break;
                case Direction.RotateCCW:
                case Direction.RotateCW:
                    motorID = e_CanMotorID.CanMotorID_Rotate;
                    res = MoveController.Instance.RoateClipper( dir == Direction.RotateCCW ? 0 : 360);
                    break;
                case Direction.ClampOff:
                case Direction.ClampOn:
                    motorID = e_CanMotorID.CanMotorID_Clipper;
                    res = MoveController.Instance.MoveClipperAtSpeed(_eARM.右臂, dir == Direction.ClampOff ? 0 : 20, 10);
                    break;
            }
            ThrowIfErrorHappened(res);
        }

        void ThrowIfErrorHappened(e_RSPErrorCode res)
        {
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(res.ToString());
        }

        public void MoveClipper(double degree, double clipWidth)
        {
            var res = MoveController.Instance.MoveClipper(clipWidth);
            ThrowIfErrorHappened(res);
            res = MoveController.Instance.RoateClipper(degree);
            ThrowIfErrorHappened(res);
        }

        public void GetClipperInfo(ref double degree, ref double clipWidth)
        {
            var res = MoveController.Instance.GetClipperInfo(ref degree, ref clipWidth);
            ThrowIfErrorHappened(res);
        }
    }
}
