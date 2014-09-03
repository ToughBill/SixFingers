//
// Todo list:
//      1. Value checking when setting the property of a labware
//      2. Serialization of labware data to XML
//      3. Missing the definition of 
//

using System;
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
        public static Labware Creat(string fromXmlFile)
        {
            if (string.IsNullOrEmpty(fromXmlFile))
                throw new ArgumentException(@"fromXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (!File.Exists(fromXmlFile))
            {
                string errorMessage = string.Format(Properties.Resources.FileNotExistsError, fromXmlFile);
                throw new ArgumentException(errorMessage);
            }

            return new Labware();
        }

        /// <summary>
        /// Create an instance of Labware from a XML node
        /// </summary>
        /// <param name="fromXmlNode"></param>
        /// <returns></returns>
        public static Labware Creat(XElement fromXmlNode)
        {
            if (fromXmlNode == null)
                throw new ArgumentNullException(@"fromXmlNode", Properties.Resources.ArgumentNullError);

            return new Labware();
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
