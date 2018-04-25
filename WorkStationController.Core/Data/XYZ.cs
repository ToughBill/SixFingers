using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class XYZ:BindableBase
    {
        double x;
        double y;
        double z;

        public XYZ(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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
    }

    
  
    public class MovingDeviceInfo
    {
        //public static bool isMoving = false;
        public static bool isJoysMoving = false;
        public static bool isKeyBoardMoving = false;
    }
}
