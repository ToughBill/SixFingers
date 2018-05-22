using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        XYZ startPosition;
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
            res = MoveController.Instance.InitClipper();
            ThrowIfErrorHappened(res);
        }

        public void Move2XYZR(ArmType armType, XYZ xyzr)
        {
            //need consider ztravel
            var err = MoveController.Instance.MoveXYZ(enumMapper[armType], xyzr.X, xyzr.Y, xyzr.Z, MoveController.defaultTimeOut);
            if (err != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(err.ToString());
        }

        public XYZ GetPosition(ArmType armType)
        {
            double x, y, z,r;
            x = y = z = r = 0;
            MoveController.Instance.GetCurrentPosition(enumMapper[armType], ref x, ref y, ref z);
            return new XYZ(x, y, z);
        }

  
        //when stop move triggers, we check whether we have moved the minium distance, 
        //if not, we move the remaining distance
        public void StopMove()
        {
            Axis axis = ConvertDir2Axis(dir);
            dir = Direction.None;
            e_RSPErrorCode res = MoveController.Instance.StopMove(enumMapper[armType], axis);
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

        private bool IsRotateOrClip(WorkstationController.Hardware.Direction dir)
        {
            return dir == WorkstationController.Hardware.Direction.ClampOff ||
                   dir == WorkstationController.Hardware.Direction.ClampOn ||
                   dir == WorkstationController.Hardware.Direction.RotateCW ||
                   dir == WorkstationController.Hardware.Direction.RotateCCW;
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
            double absPosition = 0;
            switch(dir)
            {
                case Direction.Left:
                case Direction.Right:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_x : e_CanMotorID.CanMotorID_Right_x;
                    absPosition =  dir == Direction.Left ? 0 : 700;
                    res = MoveController.Instance.StartMove(eArmType,Axis.X, speedMMPerSecond,absPosition,0);
                    Debug.WriteLine(string.Format("movex result:{0}",res.ToString()));
                    break;
                case Direction.Up:
                case Direction.Down:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_y : e_CanMotorID.CanMotorID_Right_y;
                    absPosition = dir == Direction.Down ? 400 : 0;
                    res = MoveController.Instance.StartMove(eArmType, Axis.Y, speedMMPerSecond, absPosition, 0);
                    break;
                case Direction.ZDown:
                case Direction.ZUp:
                    motorID = armType == ArmType.Liha ? e_CanMotorID.CanMotorID_Left_z : e_CanMotorID.CanMotorID_Right_z;
                    absPosition =  dir == Direction.ZUp ? 0 : 300;
                    res = MoveController.Instance.StartMove(eArmType, Axis.Z, speedMMPerSecond, absPosition, 0);
                    break;
                case Direction.RotateCCW:
                case Direction.RotateCW:
                    this.armType = ArmType.Roma;
                    motorID = e_CanMotorID.CanMotorID_Rotate;
                    absPosition =  dir == Direction.RotateCCW ? 360 : 720;
                    res = MoveController.Instance.StartMove(_eARM.右臂, Axis.R, speedMMPerSecond, absPosition, 0);
                    break;
                case Direction.ClampOff:
                case Direction.ClampOn:
                    this.armType = ArmType.Roma;
                    motorID = e_CanMotorID.CanMotorID_Clipper;
                    absPosition = dir == Direction.ClampOff ? 0 : 20;
                    res = MoveController.Instance.StartMove(_eARM.右臂, Axis.Clipper, speedMMPerSecond, absPosition, 0);
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
