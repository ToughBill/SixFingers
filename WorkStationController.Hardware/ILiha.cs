using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

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

        void GetTip(List<int> tipIDs, ref List<ITrackInfo> trackInfos);
        void DropTip(ref ITrackInfo trackInfo);
        void Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, ref List<ITrackInfo> trackInfos);

        void Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, ref List<ITrackInfo> trackInfos);

        bool IsTipMounted{get;}
    }
}
