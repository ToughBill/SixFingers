using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Linq;
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
    public class Labware : ISerialization
    {
        /// <summary>
        /// Gets or sets the name of the labware
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the labware, in millimetre(mm.)
        /// </summary>
        public int XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in millimetre(mm.)
        /// </summary>
        public int YLength { get; set; }

        /// <summary>
        /// Gets or sets the Height of the labware, in millimetre(mm.)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the radius of the well, in millimetre(mm.)
        /// </summary>
        public int WellRadius { get; set; }

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
        public int ZTravel { get; set; }

        /// <summary>
        /// Gets or sets the Z-Start value, in 1/10 millimetre
        /// </summary>
        public int ZStart { get; set; }

        /// <summary>
        /// Gets or sets the Z-Dispense value, in 1/10 millimetre
        /// </summary>
        public int ZDispense { get; set; }

        /// <summary>
        /// Gets or sets the Z-Max value, in 1/10 millimetre
        /// </summary>
        public int ZMax { get; set; }

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
        public static Labware Create(string fromXmlFile)
        {
            if (string.IsNullOrEmpty(fromXmlFile))
                throw new ArgumentException(@"fromXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (!File.Exists(fromXmlFile))
            {
                string errorMessage = string.Format(Properties.Resources.FileNotExistsError, fromXmlFile);
                throw new ArgumentException(errorMessage);
            }

            // Load Labware XML file
            XDocument labwareXmlDoc = XDocument.Load(fromXmlFile);
            XElement labwareXElement = labwareXmlDoc.Descendants("Labware").First();

            return Create(labwareXElement);
        }

        /// <summary>
        /// Create an instance of Labware from a XML node
        /// </summary>
        /// <param name="labwareXElement"></param>
        /// <returns></returns>
        internal static Labware Create(XElement labwareXElement)
        {
            if (labwareXElement == null)
                throw new ArgumentNullException(@"labwareXElement", Properties.Resources.ArgumentNullError);

            // Create a new Labware instance
            Labware labware = new Labware();

            // Get properties' value
            labware.Name = labwareXElement.Attribute("Name").Value;
            labware.XLength = int.Parse(labwareXElement.Element("XLength").Value);
            labware.YLength = int.Parse(labwareXElement.Element("YLength").Value);
            labware.Height = int.Parse(labwareXElement.Element("Height").Value);
            labware.WellRadius = int.Parse(labwareXElement.Element("WellRadius").Value);
            labware.NumberOfWellsX = int.Parse(labwareXElement.Element("NumberOfWellsX").Value);
            labware.NumberOfWellsY = int.Parse(labwareXElement.Element("NumberOfWellsY").Value);

            XElement xEle_FirstWellPosition = labwareXElement.Element("FirstWellPosition");
            int FirstWellPositionX = int.Parse(xEle_FirstWellPosition.Element("X").Value);
            int FirstWellPositionY = int.Parse(xEle_FirstWellPosition.Element("Y").Value);
            labware.FirstWellPosition = new Point(FirstWellPositionX, FirstWellPositionY);

            XElement xEle_LastWellPosition = labwareXElement.Element("LastWellPosition");
            int LastWellPositionX = int.Parse(xEle_LastWellPosition.Element("X").Value);
            int LastWellPositionY = int.Parse(xEle_LastWellPosition.Element("Y").Value);
            labware.LastWellPosition = new Point(LastWellPositionX, LastWellPositionY);

            labware.ZTravel = int.Parse(labwareXElement.Element("ZTravel").Value);
            labware.ZStart = int.Parse(labwareXElement.Element("ZStart").Value);
            labware.ZDispense = int.Parse(labwareXElement.Element("ZDispense").Value);
            labware.ZMax = int.Parse(labwareXElement.Element("ZMax").Value);

            return labware;
        }

        #region Serialization

        /// <summary>
        /// Serialize a labware to a XML file
        /// </summary>
        /// <param name="toXmlFile"></param>
        public void Serialize(string toXmlFile)
        {
            if (string.IsNullOrEmpty(toXmlFile))
                throw new ArgumentException(@"toXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (File.Exists(toXmlFile))
                File.Delete(toXmlFile);

            // Save to XML file
            XDocument labwareXMLDoc =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XComment("Labware XML definition"),
                    this.ToXElement());

            labwareXMLDoc.Save(toXmlFile);
        }

        /// <summary>
        /// Creat an XElement instance from the object
        /// </summary>
        /// <returns>XElement instance</returns>
        internal XElement ToXElement()
        {
            return new XElement("Labware", new XAttribute("Name", this.Name),
                        new XElement("XLength", this.XLength.ToString()),
                        new XElement("YLength", this.YLength.ToString()),
                        new XElement("Height", this.Height.ToString()),
                        new XElement("WellRadius", this.WellRadius.ToString()),
                        new XElement("NumberOfWellsX", this.NumberOfWellsX.ToString()),
                        new XElement("NumberOfWellsY", this.NumberOfWellsY.ToString()),
                        new XElement("FirstWellPosition", new XElement("X", this.FirstWellPosition.X.ToString()), new XElement("Y", this.FirstWellPosition.Y.ToString())),
                        new XElement("LastWellPosition", new XElement("X", this.LastWellPosition.X.ToString()), new XElement("Y", this.LastWellPosition.Y.ToString())),
                        new XElement("ZTravel", this.ZTravel.ToString()),
                        new XElement("ZStart", this.ZStart.ToString()),
                        new XElement("ZDispense", this.ZDispense.ToString()),
                        new XElement("ZMax", this.ZMax.ToString()));
        }

        #endregion
    }
}
