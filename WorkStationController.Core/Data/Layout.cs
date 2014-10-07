using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Definition of layout
    /// </summary>
    public class Layout : ISerialization
    {
        /// <summary>
        /// Name of the layout
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        private List<Carrier> carriers = new List<Carrier>();

        /// <summary>
        /// Gets the labware collection on layout
        /// </summary>
        [XmlArray("Carriers")]
        [XmlArrayItem("Carrier", typeof(Carrier), IsNullable = false)]
        public List<Carrier> Carriers
        {
            get
            {
                return this.carriers;
            }
        }

        /// <summary>
        /// Create an instance of Layout from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Layout instance</returns>
        public static Layout Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Layout>(fromXmlFile);
        }

        /// <summary>
        /// Add a Carrier
        /// </summary>
        /// <param name="carrier">Instance of Carrier</param>
        public void AddCarrier(Carrier carrier)
        {
            if(carrier == null)
            {
                throw new ArgumentNullException("carrier", "carrier must not be null.");
            }

            this.carriers.Add(carrier);
        }

        /// <summary>
        /// Remove a carrier from layout
        /// </summary>
        /// <param name="carrier">Instance of labware to remove</param>
        public void RemoveCarrier(Carrier carrier)
        {
            if (carrier == null)
            {
                throw new ArgumentNullException("carrier", "carrier must not be null.");
            }

            this.carriers.Remove(carrier);
        }

        

        #region Serialization

        /// <summary>
        /// Serializa a layout to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Layout>(toXmlFile, this);
        }

        #endregion
    }
}
