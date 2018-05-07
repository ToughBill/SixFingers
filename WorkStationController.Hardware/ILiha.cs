using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationController.Hardware
{
    public interface ILiha
    {
        void Init();
        void MoveFirstTip2AbsolutePosition(float x, float y, float z);
        void MoveFirstTipXAbs(float x);
        void MoveFirstTipYAbs(float y);
        void MoveFirstTipZAbs(float z);

        void MoveFirstTipRelativeX(float x, float speed);
        void MoveFirstTipRelativeY(float y, float speed);
        void MoveFirstTipRelativeZ(float z, float speed);

        void SetTipsDistance(float distance);
        string GetTip(List<int> tipIDs);
        string DropTip();
        string Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass);

        string Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, string liquidClass);

     
    }
}
