using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Enum of tip type
    /// </summary>
    public enum TipType
    {
        /// <summary>
        /// Fixed tip
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// Disposable tip
        /// </summary>
        Disposable = 1,
    }

    /// <summary>
    /// Data definition of tip information
    /// </summary>
    public class TipInfo
    {
        /// <summary>
        /// Gets or sets the tip type
        /// </summary>
        public TipType Type { get; set; }

        /// <summary>
        /// Gets or sets dilutor capacity
        /// </summary>
        public double DilutorCapacity { get; set; }
    }

    /// <summary>
    /// Definition of arm information
    /// </summary>
    public class ArmInfo
    {
        /// <summary>
        /// Gets or sets the ID of arm
        /// </summary>
        public byte ID { get; set; }

        /// <summary>
        /// Gets or sets the address of arm
        /// </summary>
        public byte Address { get; set; }

        private List<TipType> tipsType;
        private List<PumpInfo> pumpsInfo;

        /// <summary>
        /// Gets the number of tips
        /// </summary>
        public int TipCount 
        { 
            get
            {
                return tipsType.Count;
            }
        }

        /// <summary>
        /// Gets or sets the tip types collection
        /// </summary>
        public List<TipType> TipsType
        { 
            get
            {
                return tipsType;
            }
            set
            {
                tipsType = value;
            }
        }

        /// <summary>
        /// Gets or sets the pumps' information collection
        /// </summary>
        public List<PumpInfo> PumpsInfo
        {
            get
            {
                return pumpsInfo;
            }
            set
            {
                pumpsInfo = value;
            }
        }
    }
}
