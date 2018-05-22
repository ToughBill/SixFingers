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
        void Init();
        void Move2XYZR(ArmType armType,XYZ xyzr);

        void MoveClipper(double degree, double clipWidth);
        void GetClipperInfo(ref double degree, ref double clipWidth);
        XYZ GetPosition(ArmType armType);
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
