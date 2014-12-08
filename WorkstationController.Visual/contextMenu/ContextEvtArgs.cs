using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WorkstationController.VisualElement.ContextMenu
{
    /// <summary>
    /// clicked ware
    /// </summary>
    class ContextEvtArgs : EventArgs
    {
        public BasewareUIElement Element { get; set; }
        public Point Position { get; set; }
        public bool Need2Show { get; set; }
        public ContextEvtArgs(BasewareUIElement baseUIElement,Point position, bool need2Show = true)
        {
            Element = baseUIElement;
            Position = position;
            Need2Show = need2Show;
        }
    }
}
