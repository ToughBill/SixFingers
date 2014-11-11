using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    interface IPipettingCommmands
    {
        void GetDiti(int tipMask, string carrierLabel, DitiType ditiType, TryTimes tryTimes, int armID);
        void DropDiti(int tipMask, string wasteLabel, int armID);
        //search Diti only in the labware's racks，will try to fetch the diti at the set position
        void SetDitiPosition(string labwareLabel, DitiType ditiType, int position);

    }
   
    public enum TryTimes
    {
        Once = 1,
        ThreeTime = 3,
    }
}
