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
            MoveController.Instance.MoveXYZ(_eARM.右臂, x, y, z, MoveController.defaultTimeOut);
        }

        public WorkstationController.Core.Data.XYZR GetCurrentPosition()
        {
            //XYZR xyzr = new XYZR()
            double x,y,z,r;
            x = y = z = r = 0;
            MoveController.Instance.GetCurrentPosition(_eARM.右臂, ref x, ref y, ref z, ref r);
            return new XYZR(x, y, z, r);
        }




        public void MoveClipper(double degree, double width)
        {
            var res = MoveController.Instance.MoveClipper(degree, width);
            if(res != e_RSPErrorCode.RSP_ERROR_NONE)
            {
                throw new CriticalException(res.ToString());
            }
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
