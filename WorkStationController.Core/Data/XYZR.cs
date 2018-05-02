using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class XYZR:BindableBase
    {
        double x;
        double y;
        double z;
        double r;
        public XYZR(double x, double y, double z, double r = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                SetProperty(ref x, value);
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                SetProperty(ref y, value);
            }
        }

        public double Z
        {
            get
            {
                return z;
            }
            set
            {
                SetProperty(ref z, value);
            }
        }

        public double R
        {
            get
            {
                return r;
            }
            set
            {
                SetProperty(ref r, value);
            }
        }
    }

    
  
    public class MovingDeviceInfo
    {
        //public static bool isMoving = false;
        public static bool isJoysMoving = false;
        public static bool isKeyBoardMoving = false;
    }
}
