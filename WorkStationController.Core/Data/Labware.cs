using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;
using System.Windows.Media;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// The bottom shape of labware
    /// </summary>
    public enum BottomShape
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
        /// back ground color
        /// </summary>
        [XmlElement]
        public Color BackGroundColor { get; set; }

        /// <summary>
        /// The site on which the labware installed on the carrier
        /// </summary>
        [XmlElement]
        public int SiteID { get; set; }

        /// <summary>
        /// on which carrier the labware mounts, can be null.
        /// </summary>
        [XmlElement]
        public Carrier Carrier { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Labware()
        {
        }

        public WellsInfo WellsInfo { get; set; }
        public Dimension Dimension { get; set; }
        public ZValues ZValues { get; set; }
        public LabwareType Type { get; set; }

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




    public enum LabwareType
    {
        Tips = 0,
        Microplates = 1,
        Wash = 2,
        Waste = 3,
        Trough = 4,
        Tubes = 5,
        Misc = 6,
    }
    public class Dimension
    {
        /// <summary>
        /// Gets or sets the X-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        [XmlElement]
        public int YLength { get; set; }

        public Dimension(int x, int y)
        {
            XLength = x;
            YLength = y;
        }
    }

    public class ZValues
    {
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

        public ZValues(int travel, int start, int zDispense, int zMax)
        {
            ZTravel = travel;
            ZStart = start;
            ZDispense = zDispense;
            ZMax = zMax;
        }
    }

    //from 1000ul to 10ul
    public enum DitiType
    {
        OneK = 0,
        TwoHundred,
        Fivty,
        Ten,
    }

    public class WellsInfo
    {
        /// <summary>
        /// Gets or sets the radius of the well, in 1/10 millimetre(mm.)
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
        /// Gets or sets the bottom shape of labware
        /// </summary>
        [XmlElement]
        public BottomShape BottomShape { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public WellsInfo(Point first, Point last, int xNum, int yNum, BottomShape shape, int r)
        {
            WellRadius = r;
            NumberOfWellsX = xNum;
            NumberOfWellsY = yNum;
            FirstWellPosition = first;
            LastWellPosition = last;
            BottomShape = shape;
        }


    }

}
