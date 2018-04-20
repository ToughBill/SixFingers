using SKHardwareController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;
using WorkstationController.Hardware;

namespace WTPipetting.Hardware
{
    class HardwareController
    {
        public ILiha Liha { get; set; }
        public IRoma Roma { get; set; }
        public HardwareController(Layout layout)
        {
            Liha = new Liha(layout);
        }

      
    
        public void Init()
        {
            Liha.Init();
        }
    }
}
