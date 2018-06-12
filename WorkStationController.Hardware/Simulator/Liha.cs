using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware.Simulator
{
    public class Liha:ILiha
    {
        int tipID = 1;
        bool isTipMounted = false;
        Layout layout;
        int delayMS = 600;
        public Liha(Layout layout, string portNum)
        {
            this.layout = layout;
        }
        public void Init()
        {
            tipID = 1;
            Debug.WriteLine("Init");
        }

        public void MoveFirstTip2AbsolutePosition(float x, float y, float z)
        {
            Debug.WriteLine("MoveFirstTip2AbsolutePosition: xyz{0}{1}{2}",x,y,z);
            Delay();
        }

        public void MoveFirstTipXAbs(float x)
        {
            Debug.WriteLine("MoveFirstTipXAbs: x{0}", x);
        }

        public void MoveFirstTipYAbs(float y)
        {
            Debug.WriteLine("MoveFirstTipYAbs: y{0}", y);
        }

        public void MoveFirstTipZAbs(float z)
        {
            Debug.WriteLine("MoveFirstTipZAbs: y{0}", z);
        }

   

        public void SetTipsDistance(float distance)
        {
            Debug.WriteLine("Set tip distance:{0}", distance);
        }

        public void GetTip(List<int> tipIDs, out Core.Data.DitiTrackInfo trackInfos)
        {
            Debug.WriteLine("Get tip:{0}", tipIDs.First());
            trackInfos = new Core.Data.DitiTrackInfo("diti1", tipID++,true);
            isTipMounted = true;
            Delay();
        }

        private void Delay()
        {
            Thread.Sleep(delayMS);
        }

        public void DropTip(out Core.Data.DitiTrackInfo trackInfo)
        {
            Debug.WriteLine("Drop ID");
            trackInfo = new Core.Data.DitiTrackInfo("ditiWaste", tipID++, true, false);
            isTipMounted = false;
            Delay();
        }

        public void Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, Core.Data.LiquidClass liquidClass, out Core.Data.PipettingResult pipettingResult, string barcode = "")
        {
            Debug.WriteLine("A;{0};{1};{2}", labwareLabel, wellIDs.First(), volumes.First());
            pipettingResult = WorkstationController.Core.Data.PipettingResult.ok;
            Delay();
        }

        public void Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, Core.Data.LiquidClass liquidClass, out Core.Data.PipettingResult pipettingResult, string barcode = "")
        {
            Debug.WriteLine("D;{0};{1};{2}", labwareLabel, wellIDs.First(), volumes.First());
            pipettingResult = WorkstationController.Core.Data.PipettingResult.ok;
            Delay();
        }

        public bool IsTipMounted
        {
            get { return isTipMounted; }
        }

        public bool IsInitialized
        {
            get { return true; }
        }


        public int MaxPipettingSpeed
        {
            get { return 200; }
        }
    }
}
