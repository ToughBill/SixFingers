using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WorkstationController.Control
{
    public  class BaseEditor : UserControl
    {
        public delegate void NewInformationHandler(string s,bool isError = false);
        public NewInformationHandler newInfoHandler;
        public BaseEditor(NewInformationHandler newInfoHandler)
        {
            this.newInfoHandler = newInfoHandler;
        }
    }
}
