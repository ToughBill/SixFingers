using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    public class TeachingImplement : ITeachingController
    {
        Dictionary<ArmType, _eARM> enumMapper = new Dictionary<ArmType, _eARM>();

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
            double x, y, z, r;
            x = y = z = r = 0;
            MoveController.Instance.GetCurrentPosition(enumMapper[armType], ref x, ref y, ref z, ref r);
            return new XYZR(x, y, z, r);
        }


        public void StartMove(Direction e, int mmPerSecond)
        {
            throw new NotImplementedException();
        }

        public void StopMove()
        {
            throw new NotImplementedException();
        }
    }
}
