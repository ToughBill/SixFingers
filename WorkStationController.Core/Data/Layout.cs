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
    [Serializable]
    public class Layout 
    {
        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        protected List<Carrier> _carriers = new List<Carrier>();
        
        protected List<CarrierSkeleton> _carrierSkeletons;
        protected List<LabwareSkeleton> _labwareSkeletons;

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
            set
            {
                _carriers = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Layout()
        {
           
            CarrierSkeletons = new List<CarrierSkeleton>();
            LabwareSkeletons = new List<LabwareSkeleton>();
        }

       

        protected static List<Carrier> RestoreCarriersFromSkeleton(List<CarrierSkeleton> carrierSkeletons, List<LabwareSkeleton> labwareSkeletons)
        {
            List<Carrier> carriers = new List<Carrier>();
            foreach(CarrierSkeleton carrierSkeletonItem in carrierSkeletons)
            {
                carriers.Add(Carrier.CreateFromSkeleton(carrierSkeletonItem));
            }
             RestoreLabwares(carriers,labwareSkeletons);
             return carriers;
        }

        protected static void RestoreLabwares(List<Carrier> carriers, List<LabwareSkeleton> labwareSkeletons)
        {
            foreach (LabwareSkeleton labwareSkeleton in labwareSkeletons)
            {
                var parentCarrier = carriers.Find(x => x.GridID == labwareSkeleton.GridID);
                if (parentCarrier == null)
                {
                    //warning should be given here
                    continue;
                }
                Labware labware = Labware.CreateFromSkeleton(labwareSkeleton, parentCarrier);
                parentCarrier.Labwares.Add(labware);
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

        protected void GetSkeletonInfos()
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
            GridID = carrier.GridID;
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
        /// grid of the carrier, 1 based
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
            GridID = labware.ParentCarrier.GridID;
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
