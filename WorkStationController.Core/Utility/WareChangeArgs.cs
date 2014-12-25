using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.Utility
{
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
    public class WareEditArgs : EventArgs
    {
        /// <summary>
        /// real data to be carried
        /// </summary>
        public WareBase WareBase { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="wareBase2beEdit"></param>
        public WareEditArgs(WareBase wareBase2beEdit)
        {
            WareBase = wareBase2beEdit;
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
