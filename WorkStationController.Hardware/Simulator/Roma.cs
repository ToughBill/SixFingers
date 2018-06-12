using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware.Simulator
{
    public class Roma:IRoma
    {
        double degree;
        double width;
        XYZ xyz;
        public void Init()
        {
            degree = 0;
            width = 0;
            Debug.WriteLine("Init");
        }

        public void MoveClipper(double degree, double width)
        {
            Debug.WriteLine("MoveClipper degree:{0} width{1}",degree,width);
            this.degree = degree;
            this.width = width;
        }

        public void GetClipperInfo(ref double degree, ref double width)
        {
            degree = this.degree;
            width = this.width;
        }

        public void Move2AbsPosition(double x, double y, double z)
        {
            xyz.X = x;
            xyz.Y = y;
            xyz.Z = z;
        }

        public Core.Data.XYZ GetCurrentPosition()
        {
            return xyz;
        }

        public event EventHandler<string> onCriticalErrorHappened;


        public bool IsInitialized
        {
            get { return true; }
        }
    }
}
