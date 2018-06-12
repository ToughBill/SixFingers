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
        XYZ currentPosition = new XYZ(0,0,0);
        Stopwatch stopWatch = new Stopwatch();
        int speed_mmPerSecond = 0;
        Direction dir;
        double degree;
        double clipperWidth;
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
                case Direction.RotateCCW:
                    degree -= distance;
                    break;
                case Direction.RotateCW:
                    degree += distance;
                    break;
                case Direction.ClampOn:
                    clipperWidth -= distance;
                    break;
                case Direction.ClampOff:
                    clipperWidth += distance;
                    break;
            }
            currentPosition.X = Math.Max(0, currentPosition.X);
            currentPosition.Y = Math.Max(0, currentPosition.Y);
            currentPosition.Z = Math.Max(0, currentPosition.Z);
            degree = Math.Max(0, degree);
            clipperWidth = Math.Max(0, clipperWidth);

        }

        public void Move2XYZ(ArmType armType, Core.Data.XYZ xyz)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            double xDiff = xyz.X - currentPosition.X;
            double yDiff = xyz.Y - currentPosition.Y;
            double zDiff = xyz.Z - currentPosition.Z;
            double maxDis = Math.Max(Math.Max(xDiff, yDiff), zDiff);
            int milliSeconds = (int)(maxDis / 0.8);
            Thread.Sleep(milliSeconds);
            currentPosition = xyz;
        }

  
        public Core.Data.XYZ GetPosition(ArmType armType)
        {
            return currentPosition;
        }

        public int ZMax
        {
            get { return 250; }
        }



        public int YMax
        {
            get { return 350; }
        }


        public int GetXMax(ArmType armType)
        {
            int maxValue = 700;
            switch (armType)
            {
                case ArmType.Liha:
                    maxValue = 700;
                    break;
                case ArmType.Roma:
                    maxValue = 800;
                    break;
            }
            return maxValue;
        }

        public void StopMove()
        {
            Debug.WriteLine("stop move");
            double seconds = stopWatch.ElapsedMilliseconds < 100 ? 0.1f :  stopWatch.ElapsedMilliseconds / 1000.0f;
            seconds = Math.Round(seconds,1);
            double distance = Math.Round(speed_mmPerSecond * seconds,1);
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
                case Direction.RotateCCW:
                    degree -= distance;
                    break;
                case Direction.RotateCW:
                    degree += distance;
                    break;
                case Direction.ClampOn:
                    clipperWidth -= distance;
                    break;
                case Direction.ClampOff:
                    clipperWidth += distance;
                    break;

            }
            currentPosition.X = Math.Max(0, currentPosition.X);
            currentPosition.Y = Math.Max(0, currentPosition.Y);
            currentPosition.Z = Math.Max(0, currentPosition.Z);
            degree = Math.Max(0, degree);
            clipperWidth = Math.Max(0, clipperWidth);

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
            this.degree = degree;
            this.clipperWidth = clipWidth;
        }

        public void GetClipperInfo(ref double degree, ref double clipWidth)
        {
            degree = this.degree;
            clipWidth = this.clipperWidth;
        }

        public void Init()
        {
            Debug.WriteLine("Init");
        }


        public void GetTip()
        {
            
        }

        public void DropTip()
        {
            throw new NotImplementedException();
        }


        public int MaxPipettingSpeed
        {
            get { return 200; }
        }
    }
}
