//
// Todo list:
//      1. Value checking when setting the property of a labware
//      2. Serialization of labware data to XML
//      3. Missing the definition of 
//

using System;
using System.Windows;
using WorkStationController.Core.Utility;

namespace WorkStationController.Core.Data
{
    /// <summary>
    /// The bottom shape of labware
    /// </summary>
    public enum LabwareShape
    {
        /// <summary>
        /// Flat bottom of labware
        /// </summary>
        Flat,

        /// <summary>
        /// Rould bottom of labware
        /// </summary>
        Round,

        /// <summary>
        /// V-shape bottom of labware
        /// </summary>
        VShape
    }

    /// <summary>
    /// Data definition of a labware installed on the carrier
    /// </summary>
    public class Labware : ISerialization
    {
        /// <summary>
        /// Gets or sets the name of the labware
        /// </summary>
        public string Lable { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the labware, in millimetre(mm.)
        /// </summary>
        public double XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in millimetre(mm.)
        /// </summary>
        public double YLength { get; set; }

        /// <summary>
        /// Gets or sets the Height of the labware, in millimetre(mm.)
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the radius of the well
        /// </summary>
        public double WellRadius { get; set; }

        /// <summary>
        ///  Gets or sets number of wells in X-length
        /// </summary>
        public int NumberOfWellsX { get; set; }

        /// <summary>
        ///  Gets or sets number of wells in Y-length
        /// </summary>
        public int NumberOfWellsY { get; set; }

        /// <summary>
        /// The position of the first well (most top-left well) on labware
        /// </summary>
        public Point FirstWellPosition { get; set; }

        /// <summary>
        /// The position of the last well (most bottom-right well) on labware
        /// </summary>
        public Point LastWellPosition { get; set; }

        /// <summary>
        /// Gets or sets the Z-Travel value, in 1/10 millimetre
        /// </summary>
        int ZTravel { get; set; }

        /// <summary>
        /// Gets or sets the Z-Start value, in 1/10 millimetre
        /// </summary>
        int ZStart { get; set; }

        /// <summary>
        /// Gets or sets the Z-Dispense value, in 1/10 millimetre
        /// </summary>
        int ZDispense { get; set; }

        /// <summary>
        /// Gets or sets the Z-Max value, in 1/10 millimetre
        /// </summary>
        int ZMax { get; set; }

        /// <summary>
        /// Gets or sets the bottom shape of labware
        /// </summary>
        public LabwareShape BottomShape { get; set; }

        /// <summary>
        /// The offset (the position of the top-left coner) of the labware on carrier
        /// </summary>
        public Point PositionOnCarrier { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Labware()
        {
        }

        /// <summary>
        /// Create an instance of Labware from a XML file
        /// </summary>
        /// <param name="fromXmlFile"></param>
        /// <returns></returns>
        public static Labware Creat(string fromXmlFile)
        {
            return new Labware();
        }

        #region ISerialization

        /// <summary>
        /// Serialize a labware to a XML file
        /// </summary>
        /// <param name="toXmlFile"></param>
        public void Serialize(string toXmlFile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
