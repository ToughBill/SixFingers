using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Hardware;

namespace SKHardwareController
{
    class Roma:ArmBase,IRoma
    {
        public void Init()
        {
            
        }

        public void Move2AbsPosition(double x, double y, double z,double r)
        {
            var res = MoveController.Instance.MoveXYZ(_eARM.右臂, x, y, z, MoveController.defaultTimeOut);
            ThrowCriticalException(res);
        }

        public WorkstationController.Core.Data.XYZ GetCurrentPosition()
        {
            //XYZR xyzr = new XYZR()
            double x,y,z;
            x = y = z = 0;
            MoveController.Instance.GetCurrentPosition(_eARM.右臂, ref x, ref y, ref z);
            if (x == -1 || y == -1 || z == -1)
                throw new CriticalException("获取位置失败！");
            return new XYZ(x, y, z);
        }

        private void ThrowCriticalException(e_RSPErrorCode res)
        {
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
                throw new CriticalException(res.ToString());
        }


        public void MoveClipper(double degree, double width)
        {
            var res = MoveController.Instance.MoveClipper(degree);
            ThrowCriticalException(res);
        }

        public void Move2AbsPosition(double x, double y, double z)
        {
            var res = MoveController.Instance.MoveXYZ(_eARM.右臂, x, y, z, MoveController.defaultTimeOut);
            if (res != e_RSPErrorCode.RSP_ERROR_NONE)
            {
                throw new CriticalException(res.ToString());
            }
        }


        public void GetClipperInfo(ref double degree, ref double width)
        {
            MoveController.Instance.GetClipperInfo(ref degree, ref width);
        }
    }
}
