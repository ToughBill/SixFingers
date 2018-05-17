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

        void MoveClipper(double degree, double clipWidth);
        void GetClipperInfo(ref double degree, ref double clipWidth);
        XYZR GetPosition(ArmType armType);
        void StartMove(ArmType armType,Direction dir, int speedMMPerSecond);
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
        RotateCW,
        RotateCCW,
        ClampOn,
        ClampOff
    }
}
