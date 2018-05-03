using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Data;

namespace WorkstationController.Hardware
{
    public class TeachingControllerDelegate
    {
       
        bool registered = false;
        private static TeachingControllerDelegate instance;
        ITeachingController teachingControllerImpl;
        static public TeachingControllerDelegate Instance
        {
            get
            {
                if (instance == null)
                    instance = new TeachingControllerDelegate();
                return instance;
            }
        }

        public ITeachingController Controller
        {
            get
            {
                if (!registered)
                    throw new Exception("未注册校准控制模块！");
                return teachingControllerImpl;
            }
        }
    


        public void RegisterController(ITeachingController teachingControllerImpl)
        {
            registered = true;
            this.teachingControllerImpl = teachingControllerImpl;
        }
    }
}
