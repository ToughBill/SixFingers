using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Build-in labware types
    /// </summary>
    public enum LabwareBuildInType
    {
        Tubes16Pos13_100MM = 0,
        Plate96_05ML = 1,
        Plate24_2ML  = 2
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
    /// Data definition of a labware installed on the carrier
    /// </summary>
    [Serializable]
    public class Labware : WareBase
    {
        private int         _siteID = 1;
        private Carrier     _parentCarrier = null;
        private WellsInfo   _wellsInfo = new WellsInfo();
        private ZValues     _zValues = new ZValues();
        protected string    _label = string.Empty;
        private CoordinateInfo _coordinateInfo = null;
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
        public CoordinateInfo CoordinateOnTable
        {
            get
            {
                _coordinateInfo = GetCoordinateInfo();
                return _coordinateInfo;
            }
            set
            {
                //SetProperty(ref _coordinateInfo, value);
                _coordinateInfo = value;
                UpdateWellInfos();
            }
        }

        private void UpdateWellInfos()
        {
            Vector topLeftCurrentSite = GetTopLeftSiteVector();
            WellsInfo.FirstWellPositionX = _coordinateInfo.FirstWellPositionX - topLeftCurrentSite.X;
            WellsInfo.FirstWellPositionY = _coordinateInfo.FirstWellPositionY - topLeftCurrentSite.Y;
            WellsInfo.LastWellPositionX = _coordinateInfo.LastWellPositionX - topLeftCurrentSite.X;
            WellsInfo.LastWellPositionY = _coordinateInfo.LastWellPositionY - topLeftCurrentSite.Y;
        }

        //private void AdjustWellInfo(CoordinateInfo coordinateInfo)
        //{
        //    Vector offsetVec = GetTopLeftSiteVector();
        //    coordinateInfo.Adjust(offsetVec);
        //    _wellsInfo.UpdateCoordination(coordinateInfo);
        //}


        private Vector GetTopLeftSiteVector()
        {
            if (_parentCarrier == null)
                return new Vector(0, 0);
            
            Worktable worktable = Configurations.Instance.Worktable;
            int pinPos = (_parentCarrier.GridID - 1) * Worktable.DistanceBetweenAdjacentPins + (int)worktable.TopLeftPinPosition.X;
            int xPos = pinPos;
            int yPos = (int)worktable.TopLeftPinPosition.Y;
            if (_parentCarrier != null)
            {
                xPos = pinPos - (_parentCarrier.XOffset);  //get carrier x start pos
                yPos -= _parentCarrier.YOffset;
                int siteIndex = _siteID - 1;
                var site = _parentCarrier.Sites[siteIndex];
                xPos += (int)site.XOffset;          //get site x start pos
                yPos += (int)site.YOffset;
            }
            return new Vector(xPos, yPos);
        }

        private CoordinateInfo GetCoordinateInfo()
        {
            Vector topLeftSite = GetTopLeftSiteVector();
            if(_coordinateInfo == null || _coordinateInfo.Offset != topLeftSite)
                _coordinateInfo = new CoordinateInfo(_wellsInfo, topLeftSite);
            return _coordinateInfo;
            
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
                throw new Exception(string.Format("Cannot find the specified labware: ", labwareTraitItem.TypeName));
            var newLabware = (Labware)baseLabware.Clone();
            newLabware.Label = labwareTraitItem.Label;
            newLabware.SiteID = labwareTraitItem.SiteID;
            newLabware.ParentCarrier = parentCarrier;
            return newLabware;
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
    /// for binding to UI element
    /// </summary>
    public class CoordinateInfo : BindableBase
    {


        private WellsInfo _wellsInfo;
        private Vector _offsetVec;

        public CoordinateInfo(WellsInfo wellsInfo,Vector offsetVec)
        {
            _wellsInfo = wellsInfo;
            _offsetVec = offsetVec;
        }

        public Vector Offset
        {
            get
            {
                return _offsetVec;
            }
        }

        /// <summary>
        /// Gets or sets the X position of first well
        /// </summary>
        public double FirstWellPositionX
        {
            get
            {
                return _wellsInfo.FirstWellPositionX + _offsetVec.X;
            }
            set
            {
                _wellsInfo.FirstWellPositionX = value - _offsetVec.X;
            }
        }

        /// <summary>
        /// Gets or sets the Y position of first well
        /// </summary>
        public double FirstWellPositionY
        {
            get
            {
                return _wellsInfo.FirstWellPositionY + _offsetVec.Y;
            }
            set
            {
                _wellsInfo.FirstWellPositionY = value - _offsetVec.Y;
            }
        }

        /// <summary>
        /// Gets or sets the X position of last well
        /// </summary>
        public double LastWellPositionX
        {
            get
            {
                return _wellsInfo.LastWellPositionX + _offsetVec.X;
            }
            set
            {
                _wellsInfo.LastWellPositionX = value - _offsetVec.X;
            }
        }

        /// <summary>
        /// Gets or sets the Y position of last well
        /// </summary>
        public double LastWellPositionY
        {
            get
            {
                return _wellsInfo.LastWellPositionY + _offsetVec.Y;
            }
            set
            {
                _wellsInfo.LastWellPositionY = value - _offsetVec.Y;
            }
        }
 
    }

    /// <summary>
    /// The well information on the labware
    /// </summary>
    [Serializable]
    public class WellsInfo : BindableBase, ICloneable
    {
        private int         _wellRadius;
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
        public int WellRadius
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
                         int r)
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

        internal void UpdateCoordination(CoordinateInfo coordinateInfo)
        {
            FirstWellPositionX = coordinateInfo.FirstWellPositionX;
            FirstWellPositionY = coordinateInfo.FirstWellPositionY;
            LastWellPositionX = coordinateInfo.LastWellPositionX;
            LastWellPositionY = coordinateInfo.LastWellPositionY;
        }
    }
}
