using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Carrier installed on worktable
    /// </summary>
    [Serializable]
    public class Carrier : ISerialization
    {
        /// <summary>
        /// Labwares on the carrier
        /// </summary>
        private List<Labware> labwares = new List<Labware>();

        /// <summary>
        /// Gets or sets the lable of the carrier
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the carrier, in millimetre(mm.)
        /// </summary>
        [XmlAttribute]
        public int XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the carrier, in millimetre(mm.)
        /// </summary>
        [XmlAttribute]
        public int YLength { get; set; }

        /// <summary>
        /// Allowed type of labware 
        /// </summary>
        [XmlAttribute]
        public int AllowedLabwareType { get; set; }

        /// <summary>
        /// The maximum number of labware installed on the carrier
        /// </summary>
        [XmlAttribute]
        public int AllowedLabwareNumber { get; set; }

        /// <summary>
        /// The offset (the position of the top-left coner) of the carrier on worktable
        /// </summary>
        [XmlElement]
        public Point PositionOnWorktable { get; set; }

        /// <summary>
        /// Gets the labwares on the carrier
        /// </summary>
        [XmlArray("Labwares")]
        [XmlArrayItem("Labware", typeof(Labware), IsNullable = false)]
        public List<Labware> Labwares
        {
            get
            {
                return this.labwares;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Carrier()
        {
        }

        /// <summary>
        /// Create an instance of Carrier from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Carrier instance</returns>
        public static Carrier Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Carrier>(fromXmlFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labware"></param>
        public void AddLabware(Labware labware)
        {
            if (labware == null)
            {
                throw new ArgumentNullException("labware", "labware must not be null.");
            }

            this.labwares.Add(labware);
        }

        /// <summary>
        /// Remove a labware from carrier
        /// </summary>
        /// <param name="labware">Instance of labware to remove</param>
        public void RemoveCarrier(Labware labware)
        {
            if (labware == null)
            {
                throw new ArgumentNullException("carrier", "carrier must not be null.");
            }

            this.labwares.Remove(labware);
        }

        /// <summary>
        /// Remove a labware from carrier by lable
        /// </summary>
        /// <param name="labwareName">Lable of labware to remove</param>
        public void RemoveLabware(string labwareName)
        {
            Labware labware = this.labwares.Find(l => l.Name == labwareName);

            if(labware != null)
            {
                this.labwares.Remove(labware);
            }
        }

        #region Serialization

        /// <summary>
        /// Serialize to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for serialization</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Carrier>(toXmlFile, this);
        }

        #endregion
    }
}
