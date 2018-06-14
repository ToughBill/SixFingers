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

        int MaxPipettingSpeed
        {
            get;
        }
        void SetTipsDistance(float distance);

        void GetTip(List<int> tipIDs, DitiType ditiType, out DitiTrackInfo trackInfos);
        void DropTip(out DitiTrackInfo trackInfo);
        void Aspirate(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, out PipettingResult pipettingResult, string barcode = "");

        void Dispense(string labwareLabel, List<int> wellIDs, List<double> volumes, LiquidClass liquidClass, out PipettingResult pipettingResult, string barcode = "");

        bool IsTipMounted{get;}

        bool IsInitialized { get; }

    }
}
