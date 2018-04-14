using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

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
        protected DitiInfo _ditiInfo;
        private string _saveName;
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


        //[XmlAttribute]
        //public string Name
        //{
        //    get
        //    {
        //        return _name;
        //    }
        //    set
        //    {
        //        SetProperty(ref _name, value);
        //    }
        //}

        /// <summary>
        /// tips info
        /// </summary>
        [XmlElement]
        public DitiInfo DitiInfo
        {
            get
            {
                return _ditiInfo;
            }
            set
            {
                SetProperty(ref _ditiInfo, value);
            }
        }


        public static Layout Create(string fromXmlFile)
        {
            Layout layout = SerializationHelper.Deserialize<Layout>(fromXmlFile);
            layout._carriers = RestoreCarriersFromTrait(layout._carrierTraits, layout._labwareTraits);
            //ConstrainTipInfo(recipe);
            return layout;
        }

        /// <summary>
        /// labware reference info
        /// </summary>
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
        /// Gets the carrier collection on layout, don't serialize this
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
            _ditiInfo = new DitiInfo();
        }

        public override void Serialize(string toXmlFile)
        {
            GetTraitsInfo();
            ConstrainTipInfo(this);
            SerializationHelper.Serialize(toXmlFile, this);
        }
        internal static void ConstrainTipInfo(Layout layout)
        {
            List<DitiInfoItem> ditiInfoItems = new List<DitiInfoItem>();
            DitiInfo ditiInfo = null;
            foreach (Carrier carrier in layout.Carriers )
            {
                if (carrier.TypeName == "DitiCarrier")
                {

                    string currentDiti = "";
                    if (carrier.Labwares.Count > 0)
                        currentDiti = carrier.Labwares[0].Label;
                    foreach (Labware labware in carrier.Labwares)
                    {
                        //LabwareTraits.Add(new LabwareTrait(labware));
                        DitiInfoItem ditiItem = new DitiInfoItem(labware.Label, 96);
                        ditiInfoItems.Add(ditiItem);
                    }
                    if (ditiInfo == null)
                        ditiInfo = new DitiInfo(currentDiti, ditiInfoItems);
                    //continue;
                }
            }
            layout.DitiInfo = ditiInfo;
        }
        internal static void ConstrainTipInfo_back(Layout layout)
        {
            var tipsInfo = layout.DitiInfo.DitiInfoItems;
            List<DitiInfoItem> constrainedTipsInfo = new List<DitiInfoItem>();
            if (tipsInfo == null)
                return;
            //add value for the diti labware without this information
            foreach (var labaretraits in layout._labwareTraits)
            {
                if (!tipsInfo.Exists( x=>x.label ==  labaretraits.Label))
                {
                    constrainedTipsInfo.Add( new DitiInfoItem(labaretraits.Label, 96));
                }
            }
            foreach (var tipInfo in tipsInfo)
            {
                LabwareTrait labwareTrait = layout._labwareTraits.Find(x => x.Label == tipInfo.label);
                if (Labware.IsDiti(labwareTrait.TypeName))
                {
                    constrainedTipsInfo.Add(tipInfo);
                }
            }

            layout.DitiInfo.DitiInfoItems = constrainedTipsInfo;
            if (layout.DitiInfo.CurrentDitiLabware == "")
            {
                if (constrainedTipsInfo.Count == 0)
                    return;
                foreach(var tipInfo in constrainedTipsInfo)
                {
                    layout.DitiInfo.CurrentDitiLabware = tipInfo.label;
                    break;
                }
            }

        }


        protected static List<Carrier> RestoreCarriersFromTrait(List<CarrierTrait> carrierTraits, List<LabwareTrait> labwareTraits)
        {
            List<Carrier> carriers = new List<Carrier>();
            foreach(CarrierTrait carrierSkeletonItem in carrierTraits)
            {
                var carrier = Carrier.CreateFromTrait(carrierSkeletonItem); 
                if(carrier != null)
                    carriers.Add(carrier);
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
                if(labware != null)
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

        /// <summary>
        /// the name would be used in saveing and loading
        /// </summary>
        [XmlElement]
        public override string SaveName
        {
            get
            {
                return _saveName;
            }
            set
            {
                SetProperty(ref _saveName, value);
            }
        }

        public override string TypeName
        {
            get
            {
                return "Layout";
            }
        }

        public override void DoExtraWork()
        {
            base.DoExtraWork();
            _carriers = RestoreCarriersFromTrait(_carrierTraits, _labwareTraits);
        }

        public Labware FindLabware(string label)
        {
            Labware theOne;
            foreach(var carrier in _carriers)
            {
                theOne = carrier.Labwares.Find(x => x.Label == label);
                if (theOne != null)
                    return theOne;
            }
            return null;
        }
        public Labware FindLabwareByType(string TypeName)
        {
            Labware theOne;
            foreach (var carrier in _carriers)
            {
                theOne = carrier.Labwares.Find(x => x.TypeName == TypeName);
                if (theOne != null)
                    return theOne;
            }
            return null;
        }

        public Carrier FindCarrierByLabware(string label)
        {
            Labware theOne;
            foreach(var carrier in _carriers)
            {
                theOne = carrier.Labwares.Find(x => x.Label == label);
                if (theOne != null)
                    return carrier;
            }
            return null;
        }
        public System.Windows.Point GetPosition(Carrier carrier, Labware labware, int wellID)
        {
            double dxCarrier = carrier.XOffset;
            double dyCarrier = carrier.YOffset;

            //txtInfo.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            //new Action(
            //delegate()
            //{
            //    //txtInfo.Text += strMessage + "\r\n";
            //    labware.CalculatePositionInLayout();
            //}
            //));
            //var position = labware.GetPosition(wellID);
            //position.X += dxCarrier;
            //position.Y += dyCarrier;
            var position = labware.GetAbsPosition(wellID);            
            return position;
        }
        public System.Windows.Point GetDitiPosition()
        {
            var labware = FindLabware("Waste");
            if (labware == null)
                throw new NoLabwareException("Waste");
            return labware.GetPosition(1);
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
