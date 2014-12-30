using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Definition of layout
    /// </summary>
    [Serializable]
    public class Layout : PipettorElement
    {
        /// <summary>
        /// Carrier collection on layout
        /// </summary>
        protected List<Carrier> _carriers = new List<Carrier>();
        protected List<CarrierTrait> _carrierTraits;
        protected List<LabwareTrait> _labwareTraits;

        /// <summary>
        /// carrier reference info
        /// </summary>
        [XmlArray("CarrierTraits")]
        [XmlArrayItem("CarrierTrait", typeof(CarrierTrait), IsNullable = false)]
        public List<CarrierTrait> CarrierTraits
        {
            get
            {
                return _carrierTraits;
            }
            set
            {
                SetProperty(ref _carrierTraits, value);
            }
        }
        /// <summary>
        /// labware reference info
        /// </summary>
        [XmlArray("LabwareTraits")]
        [XmlArrayItem("LabwareTrait", typeof(LabwareTrait), IsNullable = false)]
        public List<LabwareTrait> LabwareTraits
        {
            get
            {
                return _labwareTraits;
            }
            set
            {
                SetProperty(ref _labwareTraits, value);
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
                SetProperty(ref _carriers, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Layout()
        {
            CarrierTraits = new List<CarrierTrait>();
            LabwareTraits = new List<LabwareTrait>();
        }

        protected static List<Carrier> RestoreCarriersFromTrait(List<CarrierTrait> carrierTraits, List<LabwareTrait> labwareTraits)
        {
            List<Carrier> carriers = new List<Carrier>();
            foreach(CarrierTrait carrierSkeletonItem in carrierTraits)
            {
                carriers.Add(Carrier.CreateFromSkeleton(carrierSkeletonItem));
            }
             RestoreLabwares(carriers,labwareTraits);
             return carriers;
        }

        protected static void RestoreLabwares(List<Carrier> carriers, List<LabwareTrait> labwareTraits)
        {
            foreach (LabwareTrait labwareTrait in labwareTraits)
            {
                var parentCarrier = carriers.Find(x => x.GridID == labwareTrait.GridID);
                if (parentCarrier == null)
                {
                    //warning should be given here
                    continue;
                }
                Labware labware = Labware.CreateFromTrait(labwareTrait, parentCarrier);
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

        protected void GetTraitsInfo()
        {
            CarrierTraits.Clear();
            LabwareTraits.Clear();
            foreach (Carrier carrier in _carriers)
            {
                CarrierTraits.Add(new CarrierTrait(carrier));
                foreach(Labware labware in carrier.Labwares)
                    LabwareTraits.Add(new LabwareTrait(labware));
            }
        }
    }


    /// <summary>
    /// basic info to create the carrier
    /// </summary>
    public class CarrierTrait
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="carrier"></param>
        public CarrierTrait(Carrier carrier)
        {
            TypeName = carrier.TypeName;
            GridID = carrier.GridID;
        }

        /// <summary>
        /// make serializer happy
        /// </summary>
        public CarrierTrait()
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
    public class LabwareTrait
    {
        /// <summary>
        /// make serializer happy
        /// </summary>
        public LabwareTrait()
        {

        }

        public LabwareTrait(Labware labware)
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
