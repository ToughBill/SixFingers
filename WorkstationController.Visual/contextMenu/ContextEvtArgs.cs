using System;
using System.Windows;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement.ContextMenu
{
    /// <summary>
    /// event argument
    /// </summary>
    class ContextEvtArgs : EventArgs
    {
        public WareBase ClickedWareBase { get; set; }
        public Point Position { get; set; }
        public bool Need2Show { get; set; }
        public ContextEvtArgs(WareBase wareBase, Point position, bool need2Show = true)
        {
            ClickedWareBase = wareBase;
            Position = position;
            Need2Show = need2Show;
        }
    }
}
