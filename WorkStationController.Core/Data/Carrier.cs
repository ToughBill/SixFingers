﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Carrier installed on worktable
    /// </summary>
    public class Carrier : ISerialization
    {
        /// <summary>
        /// Labwares on the carrier
        /// </summary>
        private Dictionary<string, Labware> labwares = new Dictionary<string, Labware>();

        /// <summary>
        /// Gets or sets the lable of the carrier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the carrier, in millimetre(mm.)
        /// </summary>
        public int XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the carrier, in millimetre(mm.)
        /// </summary>
        public int YLength { get; set; }

        /// <summary>
        /// Allowed type of labware 
        /// </summary>
        public int AllowedLabwareType { get; set; }

        /// <summary>
        /// The maximum number of labware installed on the carrier
        /// </summary>
        public int AllowedLabwareNumber { get; set; }

        /// <summary>
        /// The offset (the position of the top-left coner) of the carrier on worktable
        /// </summary>
        public Point PositionOnWorktable { get; set; }

        /// <summary>
        /// Gets the labwares on the carrier
        /// </summary>
        public ObservableCollection<Labware> Labwares
        {
            get
            {
                return new ObservableCollection<Labware>(this.labwares.Values);
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
        /// <param name="fromXmlFile"></param>
        /// <returns></returns>
        public static Carrier Creat(string fromXmlFile)
        {
            if (string.IsNullOrEmpty(fromXmlFile))
                throw new ArgumentException(@"fromXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (!File.Exists(fromXmlFile))
            {
                string errorMessage = string.Format(Properties.Resources.FileNotExistsError, fromXmlFile);
                throw new ArgumentException(errorMessage);
            }

            return new Carrier();
        }

        /// <summary>
        /// Create an instance of Carrier from a XML node
        /// </summary>
        /// <param name="fromXmlNode"></param>
        /// <returns></returns>
        public static Carrier Creat(XmlNode fromXmlNode)
        {
            if (fromXmlNode == null)
                throw new ArgumentNullException(@"fromXmlNode", Properties.Resources.ArgumentNullError);

            return new Carrier();
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

            if (this.labwares.ContainsKey(labware.Name))
            {
                throw new ArgumentException(string.Format("Labware - ({0}) already exists.", labware.Name), "labware");
            }

            this.labwares.Add(labware.Name, labware);
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

            this.labwares.Remove(labware.Name);
        }

        /// <summary>
        /// Remove a labware from carrier by lable
        /// </summary>
        /// <param name="labwareLable">Lable of labware to remove</param>
        public void RemoveLabware(string labwareLable)
        {
            this.labwares.Remove(labwareLable);
        }

        #region ISerialization

        /// <summary>
        /// Serialize to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for serialization</param>
        public void Serialize(string toXmlFile)
        {
            if (string.IsNullOrEmpty(toXmlFile))
                throw new ArgumentException(@"toXmlFile", Properties.Resources.FileNameArgumentError);

            // If file already exists, delete it
            if (File.Exists(toXmlFile))
                File.Delete(toXmlFile);

            // Save to XML file
            XDocument carrierXMLDoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"), 
                new XComment("Carrier XML definition"),
                this.ToXElement());

            carrierXMLDoc.Save(toXmlFile);
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
            // Save to XML file
            var labwaresAsXElement = from labware in this.Labwares
                                     select
                                     labware.ToXElement();

            XElement carrierXElement = new XElement("Carrier", new XAttribute("Name", this.Name),
                                        new XAttribute("XLength", this.XLength.ToString()),
                                        new XAttribute("YLength", this.YLength.ToString()),
                                        new XAttribute("AllowedLabwareType", this.AllowedLabwareType.ToString()),
                                        new XAttribute("AllowedLabwareNumber", this.AllowedLabwareNumber.ToString()),
                                        new XAttribute("PositionOnWorktableX", this.PositionOnWorktable.X.ToString()),
                                        new XAttribute("PositionOnWorktableY", this.PositionOnWorktable.Y.ToString()),
                                        labwaresAsXElement);

            return carrierXElement;
        }

        #endregion
    }
}
