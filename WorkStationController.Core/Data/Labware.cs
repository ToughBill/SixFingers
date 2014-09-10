using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
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
    [Serializable]
    public class Labware : ISerialization
    {
        /// <summary>
        /// Gets or sets the name of the labware
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the labware, in millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int YLength { get; set; }

        /// <summary>
        /// Gets or sets the Height of the labware, in millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the radius of the well, in millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int WellRadius { get; set; }

        /// <summary>
        ///  Gets or sets number of wells in X-length
        /// </summary>
        [XmlElement]
        public int NumberOfWellsX { get; set; }

        /// <summary>
        ///  Gets or sets number of wells in Y-length
        /// </summary>
        [XmlElement]
        public int NumberOfWellsY { get; set; }

        /// <summary>
        /// The position of the first well (most top-left well) on labware
        /// </summary>
        [XmlElement]
        public Point FirstWellPosition { get; set; }

        /// <summary>
        /// The position of the last well (most bottom-right well) on labware
        /// </summary>
        [XmlElement]
        public Point LastWellPosition { get; set; }

        /// <summary>
        /// Gets or sets the Z-Travel value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZTravel { get; set; }

        /// <summary>
        /// Gets or sets the Z-Start value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZStart { get; set; }

        /// <summary>
        /// Gets or sets the Z-Dispense value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZDispense { get; set; }

        /// <summary>
        /// Gets or sets the Z-Max value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZMax { get; set; }

        /// <summary>
        /// Gets or sets the bottom shape of labware
        /// </summary>
        [XmlElement]
        public LabwareShape BottomShape { get; set; }

        /// <summary>
        /// The offset (the position of the top-left coner) of the labware on carrier
        /// </summary>
        [XmlElement]
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
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Labware instance</returns>
        public static Labware Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Labware>(fromXmlFile);
        }

        #region Serialization

        /// <summary>
        /// Serialize a labware to a XML file
        /// </summary>
        /// <param name="toXmlFile"></param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Labware>(toXmlFile, this);
        }

        #endregion
    }
}
