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
        private int x;
        private int y;
        private int z;
        private int r;
        public int X
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

        public int Y
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


        public int Z
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


        public int R
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

        public ROMAPosition(string id,int x, int y ,int z, int r)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }
    }
}
