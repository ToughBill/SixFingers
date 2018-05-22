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
    public class TeachingControllerSimulator : ITeachingController
    {
        XYZR currentPosition = new XYZR(0,0,0);
        Stopwatch stopWatch = new Stopwatch();
        int speed_mmPerSecond = 0;
        Direction dir;
        System.Timers.Timer updatePositionTimer = new System.Timers.Timer(200);
        public void Init(string sPort)
        {
            Debug.WriteLine("Init");
         
        }
        public TeachingControllerSimulator()
        {
            updatePositionTimer.Elapsed += updatePositionTimer_Elapsed;
        }
        void updatePositionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("timer updated");
            double distance = speed_mmPerSecond * updatePositionTimer.Interval/1000;
            distance = Math.Round(distance, 1);
            switch(dir)
            {
                case Direction.Up:
                    currentPosition.Y += distance;
                    break;
                case Direction.Down:
                    currentPosition.Y -= distance;
                    break;
                case Direction.Left:
                    currentPosition.X -= distance;
                    break;
                case Direction.Right:
                    currentPosition.X += distance;
                    break;
                case Direction.ZUp:
                    currentPosition.Z += distance;
                    break;
                case Direction.ZDown:
                    currentPosition.Z -= distance;
                    break;
            }
        }

        public void Move2XYZR(ArmType armType, Core.Data.XYZR xyzr)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            double xDiff = xyzr.X - currentPosition.X;
            double yDiff = xyzr.Y - currentPosition.Y;
            double zDiff = xyzr.Z - currentPosition.Z;
            double maxDis = Math.Max(Math.Max(xDiff, yDiff), zDiff);
            int milliSeconds = (int)(maxDis / 0.8);
            Thread.Sleep(milliSeconds);
            currentPosition = xyzr;
        }

  
        public Core.Data.XYZR GetPosition(ArmType armType)
        {
            return currentPosition;
        }



        public void StopMove()
        {
            Debug.WriteLine("stop move");
            double seconds = stopWatch.ElapsedMilliseconds < 100 ? 0.1f :  stopWatch.ElapsedMilliseconds / 1000.0f;
            seconds = Math.Round(seconds,1);
            double distance = speed_mmPerSecond * seconds;
            Debug.WriteLine(string.Format("moved: {0} mm",distance));
            updatePositionTimer.Stop();
            switch (dir)
            {
                case Direction.Up:
                    currentPosition.Y += distance;
                    break;
                case Direction.Down:
                    currentPosition.Y -= distance;
                    break;
                case Direction.Left:
                    currentPosition.X -= distance;
                    break;
                case Direction.Right:
                    currentPosition.X += distance;
                    break;
                case Direction.ZUp:
                    currentPosition.Z += distance;
                    break;
                case Direction.ZDown:
                    currentPosition.Z -= distance;
                    break;
            }

        }





        public void StartMove(ArmType armType, Direction dir, int speedMMPerSecond)
        {
            updatePositionTimer.Start();
            Debug.WriteLine(string.Format("start move at:{0}", dir.ToString()));
            stopWatch.Restart();
            this.speed_mmPerSecond = speedMMPerSecond;
            this.dir = dir;
        }


        public void MoveClipper(double degree, double clipWidth)
        {
            throw new NotImplementedException();
        }

        public void GetClipperInfo(ref double degree, ref double clipWidth)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
