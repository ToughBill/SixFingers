//using SKHardwareController;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Hardware;
using WorkstationController.Hardware.Simulator;

namespace WTPipetting.Hardware
{
    class HardwareController
    {
        public ILiha Liha { get; set; }
        public IRoma Roma { get; set; }
        public HardwareController(Layout layout)
        {
            string portNum = ConfigurationManager.AppSettings["PortName"];
            Liha = new Liha(layout, portNum);
            Roma = new Roma();

        }
    
        
    }
}
