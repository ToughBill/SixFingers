using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WorkstationController.Control
{
    public class BaseEditor:UserControl
    {
        public delegate void NewInformationHandler(string info, bool isError = false);
        public NewInformationHandler newInfoHandler;
        public BaseEditor(NewInformationHandler newInfoHandler)
            :base()
        {
            // TODO: Complete member initialization
            this.newInfoHandler = newInfoHandler;
        }
    }
}
