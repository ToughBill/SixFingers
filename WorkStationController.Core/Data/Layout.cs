using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
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
        public string Name { get; set; }

        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        private Dictionary<string, Carrier> carriers = new Dictionary<string, Carrier>();

        /// <summary>
        /// Gets the labware collection on layout
        /// </summary>
        public ObservableCollection<Carrier> Carriers
        {
            get
            {
                return new ObservableCollection<Carrier>(this.carriers.Values);
            }
        }

        /// <summary>
        /// Create an instance of Layout from a XML file
        /// </summary>
        /// <param name="fromXmlFile"></param>
        /// <returns></returns>
        public static Layout Creat(string fromXmlFile)
        {
            if (string.IsNullOrEmpty(fromXmlFile))
                throw new ArgumentException(@"fromXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (!File.Exists(fromXmlFile))
            {
                string errorMessage = string.Format(Properties.Resources.FileNotExistsError, fromXmlFile);
                throw new ArgumentException(errorMessage);
            }

            return new Layout();
        }

        /// <summary>
        /// Create an instance of Layout from a XML node
        /// </summary>
        /// <param name="fromXmlNode"></param>
        /// <returns></returns>
        public static Layout Creat(XmlNode fromXmlNode)
        {
            if (fromXmlNode == null)
                throw new ArgumentNullException(@"fromXmlNode", Properties.Resources.ArgumentNullError);

            return new Layout();
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

            if(this.carriers.ContainsKey(carrier.Name))
            {
                throw new ArgumentException(string.Format("Carrier - ({0}) already exists.", carrier.Name), "carrier");
            }

            this.carriers.Add(carrier.Name, carrier);
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

            this.carriers.Remove(carrier.Name);
        }

        /// <summary>
        /// Remove a carrier from layout by lable
        /// </summary>
        /// <param name="labwareLable">Lable of labware to remove</param>
        public void RemoveLabware(string labwareLable)
        {
            this.carriers.Remove(labwareLable);
        }

        #region ISerialization

        /// <summary>
        /// Serializa a layout to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file</param>
        public void Serialize(string toXmlFile)
        {
            if (string.IsNullOrEmpty(toXmlFile))
                throw new ArgumentException(@"toXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (File.Exists(toXmlFile))
                File.Delete(toXmlFile);

            // Save to XML file
            XDocument layoutXMLDoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XComment("Layout XML definition"),
                this.ToXElement());

            layoutXMLDoc.Save(toXmlFile);
        }

        /// <summary>
        /// Serialize object to an XML node
        /// </summary>
        /// <param name="toXmlNode">XML node for saving object</param>
        public void Seialize(XmlNode toXmlNode)
        {
            if (toXmlNode == null)
                throw new ArgumentNullException(@"toXmlNode", Properties.Resources.ArgumentNullError);
        }

        internal XElement ToXElement()
        {
            var carriersAsXElement = from carrier in this.Carriers
                                     select
                                     carrier.ToXElement();

            XElement layoutXElement = new XElement("Layout", new XAttribute("Name", this.Name),
                carriersAsXElement);

            return layoutXElement;
        }

        #endregion
    }
}
