using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    public interface IRoma
    {
        void Init();
        void MoveClipper(double degree, double width);
        void GetClipperInfo(ref double degree, ref double width);
        void Move2AbsPosition(double x, double y, double z);
        XYZ GetCurrentPosition();

        event EventHandler<string> onCriticalErrorHappened;

        bool IsInitialized { get; }
    }
}
