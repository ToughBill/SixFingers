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

        float OpenWidth { get; set; }
        float ClipWidth { get; set; }

        void Open();
        void Clip();
        void Rotate(float angle);

        void Move2AbsPosition(float x, float y, float z);

        XYZR GetCurrentPosition();
        bool IsOpen { get; set; }
   

    }
}
