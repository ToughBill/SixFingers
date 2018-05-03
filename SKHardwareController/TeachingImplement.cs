using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    class TeachingImplement : ITeachingController
    {
        Dictionary<ArmType, _eARM> enumMapper = new Dictionary<ArmType, _eARM>();
        public void Init(string sPort)
        {
            enumMapper.Add(ArmType.Liha, _eARM.左臂);
            enumMapper.Add(ArmType.Roma, _eARM.右臂);
            MoveController.Instance.Init(sPort);
            MoveController.Instance.MoveHome();
        }

        public void Move2XYZR(ArmType armType, XYZR xyzr)
        {
            //need consider ztravel
            var err = MoveController.Instance.MoveXYZR(enumMapper[armType], xyzr.X, xyzr.Y, xyzr.Z, xyzr.R, MoveController.defaultTimeOut);
            if (MoveController.Instance.ErrorHappened)
                throw new CriticalException(err.ToString());
        }

        public XYZR GetPosition(ArmType armType)
        {
            double x, y, z, r;
            x = y = z = r = 0;
            MoveController.Instance.GetCurrentPosition(enumMapper[armType], ref x, ref y, ref z, ref r);
            return new XYZR(x, y, z, r);
        }


        public bool IsMoving(ArmType armType)
        {
            if (armType == ArmType.Liha)
                return MoveController.Instance.IsLihaMoving;
            else
                return MoveController.Instance.IsRomaMoving;
        }
    }
}
