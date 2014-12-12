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

    /// <summary>
    /// event arg contains labware info
    /// </summary>
    public class LabwareEditArgs : EventArgs
    {
        /// <summary>
        /// real data to be carried
        /// </summary>
        public Labware Labware { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="wareBase2beEdit"></param>
        public LabwareEditArgs(Labware wareBase2beEdit)
        {
            Labware = wareBase2beEdit;
        }

    }

    /// <summary>
    /// event arg contains carrier info
    /// </summary>
     public class CarrierEditArgs : EventArgs
    {
         /// <summary>
         /// real data to be carried
         /// </summary>
        public Carrier Carrier { get; set; }
         /// <summary>
         /// ctor
         /// </summary>
         /// <param name="wareBase2beEdit"></param>
        public CarrierEditArgs(Carrier wareBase2beEdit)
        {
            Carrier = wareBase2beEdit;
        }

    }
}
