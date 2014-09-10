using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    public class PumpInfo
    {
        public byte ID { get; set; }
        public byte address { get; set; }
        public int syringeVolumeUL { get; set; } 
        public int totalMotorSteps { get; set; }
    }
}
