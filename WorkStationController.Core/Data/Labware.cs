using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Build-in labware types
    /// </summary>
    public enum LabwareBuildInType
    {
        Tubes16Pos13_100MM = 0,
        Plate96_05ML = 1,
        Diti1000 = 2,
        Plate24_2ML  = 3
    }

    /// <summary>
    /// The bottom shape of labware
    /// </summary>
    public enum BottomShape
    {
        /// <summary>
        /// Flat bottom of labware
        /// </summary>
        Flat,

        /// <summary>
        /// Rould bottom of labware
        /// </summary>
        Round,

        /// <summary>
        /// V-shape bottom of labware
        /// </summary>
        VShape
    }

    /// <summary>
    /// Data definition of a labware mounted on carrier
    /// </summary>
    [Serializable]
    public class Labware : WareBase
    {
        private int         _siteID = 1;
        private Carrier     _parentCarrier = null;
        private WellsInfo   _wellsInfo = new WellsInfo();
        private ZValues     _zValues = new ZValues();
        protected string    _label = string.Empty;
    
        private Carrier _calibCarrier;

        private double topLeftWellXPositionInLayout;
        private double topLeftWellYPositionInLayout;
        private double bottomRightWellXPositionInLayout;
        private double bottomRightWellYPositionInLayout;

        /// <summary>
        /// The site on which the labware installed on the carrier, 1 based
        /// </summary>
        public int SiteID
        {
            get
            {
                return _siteID;
            }
            set
            {
                SetProperty(ref _siteID, value);
            }
        }

        /// <summary>
        /// Gets or sets the label of the ware
        /// </summary>
        public string Label
        {
            get { return this._label; }
            set 
            { 
                SetProperty(ref _label,value);
            }
        }

        /// <summary>
        /// On which carrier the labware mounts, can be null.
        /// </summary>
        [XmlIgnore]
        public Carrier ParentCarrier
        {
            get
            {
                return _parentCarrier;
            }
            set
            {
                //remove from the original carrier.
                if (_parentCarrier != null)
                    _parentCarrier.Labwares.Remove(this);
                _parentCarrier = value;
                if (value != null)
                {
                    SetProperty(ref _parentCarrier, value);
                }
            }
        }


        public bool IsDitiBox
        {
            get
            {
                return _typeName.Contains("Diti");
            }
        }

        [XmlIgnore]
        public override string SaveName
        {
            get
            {
                return base._typeName;
            }
            set
            {
                SetProperty(ref base._typeName,value);
            }
        }

        //we need a more eligant way to force the OnPropertyChanged event fires.
        public void Refresh()
        {
            var color = _backgroundColor;
            byte oldA = color.A;
            color.A = (byte)(255 - oldA);
            BackgroundColor = color;
            color.A = oldA;
            BackgroundColor = color;
        }


        /// <summary>
        /// The info of the wells on the ware
        /// </summary>
        public WellsInfo WellsInfo
        {
            get
            {
                return _wellsInfo;
            }
            set
            {
                SetProperty(ref _wellsInfo,value);
            }
        }

        /// <summary>
        /// the coordinate of the labware on table
        /// </summary>
        [XmlIgnore]
        public double TopLeftWellX
        {
            get
            {
                return topLeftWellXPositionInLayout;
            }
            set
            {
                SetProperty(ref topLeftWellXPositionInLayout, value);
                //UpdateWellInfos();
            }
        }

        [XmlIgnore]
        public double TopLeftWellY
        {
            get
            {
                return topLeftWellYPositionInLayout;
            }
            set
            {
                SetProperty(ref topLeftWellYPositionInLayout, value);
                //UpdateWellInfos();
            }
        }

        [XmlIgnore]
        public double BottomRightWellX
        {
            get
            {
                return bottomRightWellXPositionInLayout;
            }
            set
            {
                SetProperty(ref bottomRightWellXPositionInLayout, value);
                //UpdateWellInfos();
            }
        }

        [XmlIgnore]
        public double BottomRightWellY
        {
            get
            {
                return bottomRightWellYPositionInLayout;
            }
            set
            {
                SetProperty(ref bottomRightWellYPositionInLayout, value);
                //UpdateWellInfos();
            }
        }

       

        public void UpdateWellInfos()
        {
            Vector topLeftCurrentSite = GetTopLeftSiteVector();
            WellsInfo.FirstWellPositionX = topLeftWellXPositionInLayout - topLeftCurrentSite.X;
            WellsInfo.FirstWellPositionY = topLeftWellYPositionInLayout - topLeftCurrentSite.Y;
            WellsInfo.LastWellPositionX =  bottomRightWellXPositionInLayout - topLeftCurrentSite.X;
            WellsInfo.LastWellPositionY =  bottomRightWellYPositionInLayout - topLeftCurrentSite.Y;
        }

        
        [XmlIgnore]
        public ObservableCollection<Carrier> AllCarriers
        {
            get
            {
                return ModifyCarriersToFirstGrid(PipettorElementManager.Instance.Carriers);
                //return  ;
            }
        }

        private ObservableCollection<Carrier> ModifyCarriersToFirstGrid(ObservableCollection<Carrier> allCarriers)
        {
            ObservableCollection<Carrier> carriersOnGrid1 = new ObservableCollection<Carrier>();
            foreach(var carrier in allCarriers)
            {
                var carrierMovedToFirst = carrier.Clone() as Carrier;
                carrierMovedToFirst.GridID = 1;
                carriersOnGrid1.Add(carrierMovedToFirst);
            }
            return carriersOnGrid1;
        }



        private Vector GetTopLeftSiteVector()
        {
            var referenceCarrier = _parentCarrier;
            Worktable worktable = Configurations.Instance.Worktable;
            int needGridCnt = 0;
            if (referenceCarrier != null)
                needGridCnt =  referenceCarrier.GridID - 1;

            double pinPos = needGridCnt * Worktable.DistanceBetweenAdjacentPins + (int)worktable.TopLeftPinPosition.X;
            double xPos = pinPos;
            double yPos = worktable.TopLeftPinPosition.Y;
            if (referenceCarrier != null)
            {
                xPos = pinPos - (referenceCarrier.XOffset);  //get carrier x start pos
                yPos -= referenceCarrier.YOffset;
                int siteIndex = _siteID - 1;
                var site = referenceCarrier.Sites[siteIndex];
                xPos += (int)site.XOffset;          //get site x start pos
                yPos += (int)site.YOffset;
            }
            return new Vector(xPos, yPos);
        }

        

        /// <summary>
        /// Liquid class related
        /// </summary>
        public ZValues ZValues
        {
            get
            {
                return _zValues;
            }
            set
            {
                SetProperty(ref _zValues, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Labware()
        {
            _zValues = new ZValues();
            _dimension = new Dimension();
            _wellsInfo = new WellsInfo();
            
            if (_parentCarrier != null)
                _calibCarrier = _parentCarrier;

            
        }

        //public override void DoExtraWork()
        //{
        //    base.DoExtraWork();
        //    CalculatePositionInLayout();
        //}

        public void CalculatePositionInLayout()
        {
            Vector topLeftCurrentSite = GetTopLeftSiteVector();
            TopLeftWellX = _wellsInfo.FirstWellPositionX + topLeftCurrentSite.X;
            TopLeftWellY = _wellsInfo.FirstWellPositionY + topLeftCurrentSite.Y;
            BottomRightWellX = WellsInfo.LastWellPositionX + topLeftCurrentSite.X;
            BottomRightWellY = WellsInfo.LastWellPositionY + topLeftCurrentSite.Y;
        }



        /// <summary>
        /// the grid
        /// </summary>
        public int GridID
        {
            get
            {
                return ParentCarrier == null ? 1 : this.ParentCarrier.GridID;
            }
        }

        static public Labware CreateFromTrait(LabwareTrait labwareTraitItem, Carrier parentCarrier = null)
        {
            // TODO: Complete member initialization
            List<Labware> labwares = new List<Labware>(PipettorElementManager.Instance.Labwares);
            var baseLabware = labwares.Find(x => x.TypeName == labwareTraitItem.TypeName);
            if( baseLabware == null)
            {
                return null;
            }
                //throw new Exception(string.Format("Cannot find the specified labware: ", labwareTraitItem.TypeName));
            var newLabware = (Labware)baseLabware.Clone();
            newLabware.Label = labwareTraitItem.Label;
            newLabware.SiteID = labwareTraitItem.SiteID;
            newLabware.ParentCarrier = parentCarrier;
            return newLabware;
        }

        public Point GetPosition(int wellID)
        {
            int maxWellCnt = _wellsInfo.NumberOfWellsX * _wellsInfo.NumberOfWellsY;
            if (wellID > maxWellCnt)
                throw new WellOfoutRange(this.Label, maxWellCnt);
            int col = (wellID - 1) / _wellsInfo.NumberOfWellsY;
            int row = (wellID - 1) % _wellsInfo.NumberOfWellsY;
            return GetPosition(row, col);
        }

        public Point GetAbsPosition(int wellID)
        {
            Point point = GetPosition(wellID);
            Vector topLeftCurrentSite = GetTopLeftSiteVector();
            point.X += topLeftCurrentSite.X;
            point.Y += topLeftCurrentSite.Y;
            return point;
        }

        /// <summary>
        /// get physical position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Point GetPosition(int row, int col)
        {
            if (col >= _wellsInfo.NumberOfWellsX)
                throw new Exception("column index bigger or equal to the column count!");
            if( row >= _wellsInfo.NumberOfWellsY)
                throw new Exception("row index bigger or equal to the row count!");

            double x = 0;
            double xs = _wellsInfo.FirstWellPositionX;
            double xe = _wellsInfo.LastWellPositionX;
            if (_wellsInfo.NumberOfWellsX == 1)
            {
                x = xs;
            }
            else
            {
                double eachXUnit = (xe - xs) / (WellsInfo.NumberOfWellsX - 1);
                x =  xs + col * eachXUnit;
            }

            int ys = (int)WellsInfo.FirstWellPositionY;
            int ye = (int)WellsInfo.LastWellPositionY;

            double y = 0;
            if(_wellsInfo.NumberOfWellsY == 1)
            {
                y = ys;
            }
            else
            {
                double eachYUnit = (ye - ys) / (WellsInfo.NumberOfWellsY - 1);
                y = ys + row * eachYUnit;
            }
           
            return new Point(x, y);
        }


        /// <summary>
        /// Create an instance of Labware from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Labware instance</returns>
        public static Labware Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Labware>(fromXmlFile);
        }
        
        #region Serialization

        /// <summary>
        /// Serialize a labware to a XML file
        /// </summary>
        /// <param name="toXmlFile"></param>
        public override void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Labware>(toXmlFile, this);
        }

        #endregion

        public override object Clone()
        {
            Labware copy = new Labware();
            copy._label = "<Need a label>";
            copy._typeName = this.TypeName;
            copy._dimension = (Dimension)this.Dimension.Clone();
            copy._backgroundColor = this._backgroundColor;
            copy._parentCarrier = null;
            copy._siteID = this._siteID;
            copy._wellsInfo = (WellsInfo)this._wellsInfo.Clone();
            copy._zValues = (ZValues)this._zValues.Clone();
            return copy;
        }

        /// <summary>
        /// update the information which is common, very similar to clone
        /// </summary>
        /// <param name="exampleLabware"></param>
        public void CarryInfo(Labware exampleLabware)
        {
            _typeName = exampleLabware.TypeName;
            _dimension = exampleLabware._dimension;
            _backgroundColor = exampleLabware.BackgroundColor;
            _wellsInfo = (WellsInfo)exampleLabware.WellsInfo.Clone();
            _zValues = (ZValues)exampleLabware.ZValues.Clone();
        }


        internal static bool IsDiti(string typeName)
        {
            return typeName.Contains(strings.DitiKeyName);
        }
    }

    /// <summary>
    /// Four z-heights used in pipetting
    /// </summary>
    [Serializable]
    public class ZValues : BindableBase, ICloneable
    {
        private int _zTravel;
        private int _zStart;
        private int _zMax;
        private int _zDispense;

        /// <summary>
        /// Gets or sets the Z-Travel value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZTravel
        {
            get
            {
                return _zTravel;
            }
            set
            {
                SetProperty(ref _zTravel, value);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Start value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZStart
        {
            get
            {
                return _zStart;
            }
            set
            {
                SetProperty(ref _zStart, value);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Dispense value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZDispense
        {
            get
            {
                return _zDispense;
            }
            set
            {
                SetProperty(ref _zDispense, value);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Max value, in 1/10 millimetre
        /// </summary>
        [XmlElement]
        public int ZMax
        {
            get
            {
                return _zMax;
            }
            set
            {
                SetProperty(ref _zMax, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ZValues()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="travel"></param>
        /// <param name="start"></param>
        /// <param name="zDispense"></param>
        /// <param name="zMax"></param>
        public ZValues(int travel, int start, int zDispense, int zMax)
        {
            _zTravel = travel;
            _zStart = start;
            _zDispense = zDispense;
            _zMax = zMax;
        }

        public object Clone()
        {
            return new ZValues(this._zTravel, this._zStart, this._zDispense, this._zMax);
        }
    }

    

    /// <summary>
    /// The well information on the labware
    /// </summary>
    [Serializable]
    public class WellsInfo : BindableBase, ICloneable
    {
        private double _wellRadius;
        private int         _numberOfWellsX;
        private int         _numberOfWellsY;
        private double      _firstWellPositionX;
        private double      _firstWellPositionY;
        private double      _lastWellPositionX;
        private double      _lastWellPositionY;
        private BottomShape _bottomShape;

        /// <summary>
        /// Gets or sets the radius of the well, in 1/10 millimetre(mm.)
        /// </summary>
        public double WellRadius
        {
            get
            {
                return _wellRadius;
            }
            set
            {
                SetProperty(ref _wellRadius, value);
            }
        }

        /// <summary>
        ///  Gets or sets number of wells in X-length
        /// </summary>
        public int NumberOfWellsX
        {
            get
            {
                return _numberOfWellsX;
            }
            set
            {
                SetProperty(ref _numberOfWellsX, value);
            }
        }

        /// <summary>
        ///  Gets or sets number of wells in Y-length
        /// </summary>
        public int NumberOfWellsY
        {
            get
            {
                return _numberOfWellsY;
            }
            set
            {
                SetProperty(ref _numberOfWellsY, value);
            }
        }

        /// <summary>
        /// Gets or sets the X position of first well
        /// </summary>
        public double FirstWellPositionX
        {
            get
            {
                return _firstWellPositionX;
            }
            set
            {
                SetProperty(ref _firstWellPositionX, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y position of first well
        /// </summary>
        public double FirstWellPositionY
        {
            get
            {
                return _firstWellPositionY;
            }
            set
            {
                SetProperty(ref _firstWellPositionY, value);
            }
        }

        /// <summary>
        /// Gets or sets the X position of last well
        /// </summary>
        public double LastWellPositionX
        {
            get
            {
                return _lastWellPositionX;
            }
            set
            {
                SetProperty(ref _lastWellPositionX, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y position of last well
        /// </summary>
        public double LastWellPositionY
        {
            get
            {
                return _lastWellPositionY;
            }
            set
            {
                SetProperty(ref _lastWellPositionY, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom shape of labware
        /// </summary>
        public BottomShape BottomShape
        {
            get
            {
                return _bottomShape;
            }
            set
            {
                SetProperty(ref _bottomShape, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WellsInfo()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WellsInfo(double firstWellPosX, double firstWellPosY,
                         double lastWellPosX, double lastWellPosY,
                         int xNum, int yNum, 
                         BottomShape shape, 
                         double r)
        {
            WellRadius = r;
            NumberOfWellsX = xNum;
            NumberOfWellsY = yNum;
            FirstWellPositionX = firstWellPosX;
            FirstWellPositionY = firstWellPosY;
            LastWellPositionX = lastWellPosX;
            LastWellPositionY = lastWellPosY;
            BottomShape = shape;
        }

        public object Clone()
        {
            return new WellsInfo(this._firstWellPositionX, this._firstWellPositionY, 
                                 this._lastWellPositionX, this._lastWellPositionY, 
                                 this._numberOfWellsX, this._numberOfWellsY, 
                                 this._bottomShape, 
                                 _wellRadius);
        }

        
    }
}
