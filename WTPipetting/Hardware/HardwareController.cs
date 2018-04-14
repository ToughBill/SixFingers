using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;
using WorkstationController.Core.Managements;

namespace WTPipetting.Hardware
{
    class HardwareController
    {

        public HardwareController(Layout layout)
        {
            Roma = new Roma();
            Liha = new Liha(layout);
            
        }

        public Roma Roma { get; set; }
        public Liha Liha { get; set; }
    
        public void Init()
        {
            Liha.Init();
            //Roma.Init();
        }
    }
}
