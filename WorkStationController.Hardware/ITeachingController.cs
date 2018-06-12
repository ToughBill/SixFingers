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
        void Move2XYZ(ArmType armType,XYZ xyz);

        int MaxPipettingSpeed { get; }

        int ZMax { get; }
        int GetXMax(ArmType armType);
        int YMax { get; }

        void GetTip();

        void DropTip();
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
