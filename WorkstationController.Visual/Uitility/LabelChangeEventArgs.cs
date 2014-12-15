using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WorkstationController.VisualElement.Uitility
{
    /// <summary>
    /// label changed event
    /// </summary>
    public class LabelChangeEventArgs : EventArgs
    {
        LabwareUIElement _labwareUIElement = null;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="uiElement"></param>
        public LabelChangeEventArgs(LabwareUIElement uiElement)
        {
            Debug.WriteLine("args hashcode :{0}", uiElement.GetHashCode());
            _labwareUIElement = uiElement;
        }

        /// <summary>
        /// labware uiElement
        /// </summary>
        public LabwareUIElement LabwareUIElement
        {
            get
            {
                return _labwareUIElement;
            }
        }
    }
}
