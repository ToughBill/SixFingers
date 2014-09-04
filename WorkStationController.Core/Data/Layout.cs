﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        public static Layout Create(string fromXmlFile)
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
            XDocument layoutXmlDoc = XDocument.Load(fromXmlFile);
            XElement layoutXElement = layoutXmlDoc.Descendants("Layout").First();

            return Layout.Create(layoutXElement);
        }

        /// <summary>
        /// Create an instance of Layout from a XML node
        /// </summary>
        /// <param name="layoutXElement"></param>
        /// <returns></returns>
        public static Layout Create(XElement layoutXElement)
        {
            if (layoutXElement == null)
                throw new ArgumentNullException(@"layoutXElement", Properties.Resources.ArgumentNullError);

            Layout layout = new Layout();
            layout.Name = layoutXElement.Attribute("Name").Value;

            // Read Carrier XElements
            foreach(XElement carrierXElement in layoutXElement.Descendants("Carrier"))
            {
                Carrier carrier = Carrier.Create(carrierXElement);
                layout.AddCarrier(carrier);
            }

            return layout;
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

        #region Serialization

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
        /// Creat an XElement instance from the object
        /// </summary>
        /// <returns>XElement instance</returns>
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
