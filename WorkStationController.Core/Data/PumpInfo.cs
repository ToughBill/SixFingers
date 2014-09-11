using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of pump information
    /// </summary>
    public class PumpInfo
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public byte ID { get; set; }

        /// <summary>
        /// Gets or sets the address of the pump
        /// </summary>
        public byte Address { get; set; }

        /// <summary>
        /// Gets or sets the syringe volume, in unit of μl
        /// </summary>
        public int SyringeVolumeUL { get; set; }
 
        /// <summary>
        /// Gets or sets the total motor steps
        /// </summary>
        public int TotalMotorSteps { get; set; }
    }
}
