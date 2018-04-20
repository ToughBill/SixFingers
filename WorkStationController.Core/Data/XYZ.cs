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

    
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))

                return false; storage = value; this.OnPropertyChanged(propertyName); return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;

            if (eventHandler != null)

            { eventHandler(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }

    public class MovingDeviceInfo
    {
        //public static bool isMoving = false;
        public static bool isJoysMoving = false;
        public static bool isKeyBoardMoving = false;
    }
}
