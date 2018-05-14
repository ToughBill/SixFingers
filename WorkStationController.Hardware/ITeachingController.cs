using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    public interface ITeachingController
    {
        void Init(string sPort);
        void Move2XYZR(ArmType armType,XYZR xyzr);

       
        XYZR GetPosition(ArmType armType);
        void StartMove(Direction e, int speedMMPerSecond);
        void StopMove();
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        ZUp,
        ZDown,
        RotateLeft,
        RotateRight,
        ClampOn,
        ClampOff
    }
}
