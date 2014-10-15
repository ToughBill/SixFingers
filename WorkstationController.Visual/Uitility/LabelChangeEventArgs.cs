using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WorkstationController.VisualElement.Uitility
{
    public class LabelChangeEventArgs : EventArgs
    {
        LabwareUIElement _labwareUIElement = null;
        public LabelChangeEventArgs(LabwareUIElement uiElement)
        {
            Debug.WriteLine("args hashcode :{0}", uiElement.GetHashCode());
            _labwareUIElement = uiElement;
        }

        public LabwareUIElement LabwareUIElement
        {
            get
            {
                return _labwareUIElement;
            }
        }
    }
}
