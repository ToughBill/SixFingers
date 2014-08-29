using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WorkStationController.Core.Utility;

namespace WorkStationController.Core.Data
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
        public string Lable { get; set; }

        /// <summary>
        /// Gets or sets the X-length of the carrier, in millimetre(mm.)
        /// </summary>
        public double XLength { get; set; }

        /// <summary>
        /// Gets or sets the Y-length of the carrier, in millimetre(mm.)
        /// </summary>
        public double YLength { get; set; }

        /// <summary>
        /// The offset (the position of the top-left coner) of the carrier on worktable
        /// </summary>
        public Point PositionOnWorktable { get; set; }

        /// <summary>
        /// Gets the labwares on the carrier
        /// </summary>
        public Dictionary<string, Labware> Labwares
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
        /// 
        /// </summary>
        /// <param name="labware"></param>
        public void AddLabware(Labware labware)
        {
            if (labware == null)
            {
                throw new ArgumentNullException("labware", "labware must not be null.");
            }

            if (this.labwares.ContainsKey(labware.Lable))
            {
                throw new ArgumentException(string.Format("Labware - ({0}) already exists.", labware.Lable), "labware");
            }

            this.labwares.Add(labware.Lable, labware);
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

            this.labwares.Remove(labware.Lable);
        }

        /// <summary>
        /// Remove a labware from carrier by lable
        /// </summary>
        /// <param name="labwareLable">Lable of labware to remove</param>
        public void RemoveLabware(string labwareLable)
        {
            this.labwares.Remove(labwareLable);
        }

        /// <summary>
        /// Serialize to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for serialization</param>
        public void Serialize(string toXmlFile)
        {
 	        throw new NotImplementedException();
        }
    }
}
