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

        bool IsMoving(ArmType armType);
        XYZR GetPosition(ArmType armType);
    }
}
