using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core;

namespace WorkstationController.Core.Data
{
    public class ROMAPosition:BindableBase
    {
        private string id;
        private double x;
        private double y;
        private double z;
        private double r;
        private double clipDistance;
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

        public double ClipDistance
        {
            get
            {
                return clipDistance;
            }
            set
            {
                SetProperty(ref clipDistance, value);
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

        public string   ID
        {
            get
            {
                return id;
            }
            set
            {
                SetProperty(ref id, value);
            }
        }

        public ROMAPosition()
        {

        }

        public ROMAPosition(string id, double x, double y, double z, double r,double clipDistance)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
            this.clipDistance = clipDistance;
        }
    }
}
