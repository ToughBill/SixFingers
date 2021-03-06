﻿using System;
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
        protected List<PlateVector> _plateVectors;
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

        static Layout selectedLayout;
        public static Layout SelectedLayout
        {
            get
            {
                return selectedLayout;
            }
            set
            {
                selectedLayout = value;
            }
        }

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


        [XmlArray("PlateVectors")]
        [XmlArrayItem("PlateVector", typeof(PlateVector), IsNullable = false)]
        public List<PlateVector> PlateVectors
        {
            get
            {
                return _plateVectors;
            }
            set
            {
                SetProperty(ref _plateVectors, value);
            }
        }

        public static Layout Create(string fromXmlFile)
        {
            Layout layout = SerializationHelper.Deserialize<Layout>(fromXmlFile);
            layout._carriers = RestoreCarriersFromTrait(layout._carrierTraits, layout._labwareTraits, layout._plateVectors);
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
            GetPlateVectors();
            ConstrainTipInfo(this);
            SerializationHelper.Serialize(toXmlFile, this);
        }

        private void GetPlateVectors()
        {
            _plateVectors = new List<PlateVector>();
            foreach (Carrier carrier in _carriers )
            {
                foreach (Labware labware in carrier.Labwares)
                {
                    if(labware.PlateVector != null)
                    {
                        _plateVectors.Add(labware.PlateVector);
                    }
                }
            }
        }
        internal static void ConstrainTipInfo(Layout layout)
        {
            if (layout.DitiInfo != null 
                && layout.DitiInfo.DitiBoxInfos != null
                && layout.DitiInfo.DitiBoxInfos.Count != 0)
                return;

            DitiInfo ditiInfo = new DitiInfo();
            Dictionary<DitiType, string> currentDitis = new Dictionary<DitiType, string>();
            foreach (Carrier carrier in layout.Carriers )
            {
                if (carrier.TypeName == BuildInCarrierType.Diti.ToString())
                {

                    foreach (Labware labware in carrier.Labwares)
                    {
                        //LabwareTraits.Add(new LabwareTrait(labware));
                        DitiType ditiType = DitiBox.Parse(labware.TypeName); 
                        DitiBoxInfo ditiBoxInfo = new DitiBoxInfo(ditiType, labware.Label,96);
                        if (ditiInfo.GetCurrentLabel(ditiType) == "")
                        {
                            ditiBoxInfo.isUsing = true;
                        }
                        ditiInfo.DitiBoxInfos.Add(ditiBoxInfo);
                    }
                }
            }
            layout.DitiInfo = ditiInfo;
        }
        internal static void ConstrainTipInfo_back(Layout layout)
        {
            var ditiInfos = layout.DitiInfo.DitiBoxInfos;
            List<DitiBoxInfo> constrainedDitiInfos = new List<DitiBoxInfo>();
            if (ditiInfos == null)
                return;
            //add value for the diti labware without this information
            foreach (var labwareTrait in layout._labwareTraits)
            {
                if (!ditiInfos.Exists( x=>x.label ==  labwareTrait.Label))
                {
                    DitiType ditiType = DitiBox.Parse(labwareTrait.TypeName);
                    constrainedDitiInfos.Add(new DitiBoxInfo(ditiType,labwareTrait.Label, 96));
                }
            }
            foreach (var tipInfo in ditiInfos)
            {
                LabwareTrait labwareTrait = layout._labwareTraits.Find(x => x.Label == tipInfo.label);
                if (Labware.IsDiti(labwareTrait.TypeName))
                {
                    constrainedDitiInfos.Add(tipInfo);
                }
            }

            layout.DitiInfo.DitiBoxInfos = constrainedDitiInfos;
            if (layout.DitiInfo.DitiBoxInfos.Count == 0)
            {
                foreach(var ditiInfo in constrainedDitiInfos)
                {
                    if (layout.DitiInfo.GetCurrentLabel(ditiInfo.type) != "")
                        layout.DitiInfo.DitiBoxInfos.Add(new DitiBoxInfo(ditiInfo.type, ditiInfo.label,96));
                }
            }

        }


        protected static List<Carrier> RestoreCarriersFromTrait(List<CarrierTrait> carrierTraits,
            List<LabwareTrait> labwareTraits,List<PlateVector> plateVectors)
        {
            List<Carrier> carriers = new List<Carrier>();
            foreach(CarrierTrait carrierSkeletonItem in carrierTraits)
            {
                var carrier = Carrier.CreateFromTrait(carrierSkeletonItem); 
                if(carrier != null)
                    carriers.Add(carrier);
            }
            RestoreLabwares(carriers, labwareTraits, plateVectors);
             return carriers;
        }

        protected static void RestoreLabwares(List<Carrier> carriers, List<LabwareTrait> labwareTraits, List<PlateVector> plateVectors)
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
                {
                    var plateVector = plateVectors.Find(x => x.Name == labwareTrait.Label);
                    labware.PlateVector = plateVector;
                    parentCarrier.Labwares.Add(labware);
                }
                    

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
            _carriers = RestoreCarriersFromTrait(_carrierTraits, _labwareTraits,_plateVectors);
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

            var position = labware.GetAbsPosition(wellID);            
            return position;
        }
        public System.Windows.Point GetWastePosition()
        {
            var labware = FindLabwareByType(Labware.WasteLabel);
            if (labware == null)
                throw new NoLabwareException(Labware.WasteLabel);
            return labware.GetAbsPosition(1);
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
