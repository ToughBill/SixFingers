using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    class TeachingControllerSimulator : ITeachingController
    {
        XYZR currentPosition = new XYZR(0,0,0);
        bool isMoving = false;
        public void Init(string sPort)
        {
            Debug.WriteLine("Init");
        }

        public void Move2XYZR(ArmType armType, Core.Data.XYZR xyzr)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            isMoving = true;
            double xDiff = xyzr.X - currentPosition.X;
            double yDiff = xyzr.Y - currentPosition.Y;
            double zDiff = xyzr.Z - currentPosition.Z;
            double maxDis = Math.Max(Math.Max(xDiff, yDiff), zDiff);
            int milliSeconds = (int)(maxDis / 0.8);
            Thread.Sleep(milliSeconds);
            isMoving = false;
            currentPosition = xyzr;
        }

        public bool IsMoving(ArmType armType)
        {
            return isMoving;
        }

        public Core.Data.XYZR GetPosition(ArmType armType)
        {
            return currentPosition;
        }
    }
}
