﻿using System;
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
        /// UID of the layout
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        private List<Carrier> _carriers = new List<Carrier>();

        private List<CarrierSkeleton> _carrierSkeletons;
        private List<LabwareSkeleton> _labwareSkeletons;
        /// <summary>
        /// carrier reference info
        /// </summary>
        [XmlArray("CarrierSkeletons")]
        [XmlArrayItem("CarrierSkeleton", typeof(CarrierSkeleton), IsNullable = false)]
        public List<CarrierSkeleton> CarrierSkeletons
        {
            get
            {
                return _carrierSkeletons;
            }
            set
            {
                _carrierSkeletons = value;
            }
        }
        /// <summary>
        /// labware reference info
        /// </summary>
        [XmlArray("LabwareSkeletons")]
        [XmlArrayItem("LabwareSkeleton", typeof(LabwareSkeleton), IsNullable = false)]
        public List<LabwareSkeleton> LabwareSkeletons
        {
            get
            {
                return _labwareSkeletons;
            }
            set
            {
                _labwareSkeletons = value;
            }
        }


        /// <summary>
        /// Gets the labware collection on layout, don't serialize this
        /// </summary>
        [XmlIgnoreAttribute] 
        public List<Carrier> Carriers
        {
            get
            {
                return this._carriers;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Layout()
        {
            this.ID = Guid.NewGuid();
            CarrierSkeletons = new List<CarrierSkeleton>();
            LabwareSkeletons = new List<LabwareSkeleton>();
        }

        /// <summary>
        /// Create an instance of Layout from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Layout instance</returns>
        public static Layout Create(string fromXmlFile)
        {
           Layout layout =  SerializationHelper.Deserialize<Layout>(fromXmlFile);
           layout._carriers = RestoreCarriersFromSkeleton(layout._carrierSkeletons,layout._labwareSkeletons);
           return layout;
        }

        private static List<Carrier> RestoreCarriersFromSkeleton(List<CarrierSkeleton> carrierSkeletons, List<LabwareSkeleton> labwareSkeletons)
        {
            List<Carrier> carriers = new List<Carrier>();
            List<Labware> labwares = new List<Labware>();
            foreach(CarrierSkeleton carrierSkeletonItem in carrierSkeletons)
            {
                carriers.Add(new Carrier(carrierSkeletonItem.TypeName, carrierSkeletonItem.GridID));
            }
             foreach(LabwareSkeleton labwareSkeletonItem in labwareSkeletons)
             {
                 labwares.Add(new Labware(labwareSkeletonItem));
             }
             MountLabwaresOntoCarriers(carriers, labwares);
             return carriers;
        }

        private static void MountLabwaresOntoCarriers(List<Carrier> carriers, List<Labware> labwares)
        {
            throw new NotImplementedException();
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
            if (_carriers.Contains(carrier))
                return;
            this._carriers.Add(carrier);
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

            this._carriers.Remove(carrier);
        }

        #region Serialization

        /// <summary>
        /// Serializa a layout to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file</param>
        public void Serialize(string toXmlFile)
        {
            GetSkeletonInfos();
           
            SerializationHelper.Serialize<Layout>(toXmlFile, this);
        }

        private void GetSkeletonInfos()
        {
            CarrierSkeletons.Clear();
            LabwareSkeletons.Clear();
            foreach (Carrier carrier in _carriers)
            {
                CarrierSkeletons.Add(new CarrierSkeleton(carrier));
                foreach(Labware labware in carrier.Labwares)
                    LabwareSkeletons.Add(new LabwareSkeleton(labware));
            }
        }



        #endregion
    }


    /// <summary>
    /// basic info to create the carrier
    /// </summary>
    public class CarrierSkeleton
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="carrier"></param>
        public CarrierSkeleton(Carrier carrier)
        {
            TypeName = carrier.TypeName;
            GridID = carrier.Grid;
        }

        /// <summary>
        /// make serializer happy
        /// </summary>
        public CarrierSkeleton()
        {

        }
        /// <summary>
        ///as property
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// grid of the carrier
        /// </summary>
        public int GridID { get; set; }
    }

    /// <summary>
    /// basic info to create the labware
    /// </summary>
    public class LabwareSkeleton
    {
        /// <summary>
        /// make serializer happy
        /// </summary>
        public LabwareSkeleton()
        {

        }

        public LabwareSkeleton(Labware labware)
        {
            // TODO: Complete member initialization
            TypeName = labware.TypeName;
            GridID = labware.ParentCarrier.Grid;
            SiteID = labware.SiteID;
            Label = labware.Label;
        }
        /// <summary>
        /// as property
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// as property
        /// </summary>
        public int GridID { get; set; }
        
        /// <summary>
        /// as property
        /// </summary>
        public int SiteID { get; set; }

        /// <summary>
        /// as property
        /// </summary>
        public string Label { get; set; }
    }
}
