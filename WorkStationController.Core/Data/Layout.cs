using System;
using System.Collections.Generic;
using WorkStationController.Core.Utility;

namespace WorkStationController.Core.Data
{
    /// <summary>
    /// Definition of layout
    /// </summary>
    public class Layout : ISerialization
    {
        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        private Dictionary<string, Carrier> carriers = new Dictionary<string, Carrier>();

        /// <summary>
        /// Gets the labware collection on layout
        /// </summary>
        public Dictionary<string, Carrier> Carriers
        {
            get
            {
                return this.carriers;
            }
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

            if(this.carriers.ContainsKey(carrier.Lable))
            {
                throw new ArgumentException(string.Format("Carrier - ({0}) already exists.", carrier.Lable), "carrier");
            }

            this.carriers.Add(carrier.Lable, carrier);
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

            this.carriers.Remove(carrier.Lable);
        }

        /// <summary>
        /// Remove a carrier from layout by lable
        /// </summary>
        /// <param name="labwareLable">Lable of labware to remove</param>
        public void RemoveLabware(string labwareLable)
        {
            this.carriers.Remove(labwareLable);
        }

        /// <summary>
        /// Serializa a layout to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file</param>
        public void Serialize(string toXmlFile)
        {
            throw new NotImplementedException();
        }
    }
}
