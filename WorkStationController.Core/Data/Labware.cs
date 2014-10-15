using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;
using System.Windows.Media;
using System.ComponentModel;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Build-in labware types
    /// </summary>
    public enum LabwareBuildInType
    {
        Tubes16Pos13_100MM = 0,
        Plate96_05ML = 1
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
    public class Labware : WareBase, ISerialization, INotifyPropertyChanged, ICloneable
    {
        private Color       _backgroundColor;
        private int         _siteID = 0;
        private int         _grid = Carrier.undefinedGrid;
        private WellsInfo   _wellsInfo = new WellsInfo();
        private ZValues     _zValues = new ZValues();

        /// <summary>
        /// back ground color
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                this.OnPropertyChanged<Color>(ref this._backgroundColor, value, "BackgroundColor");
            }
        }

        /// <summary>
        /// The site on which the labware installed on the carrier
        /// </summary>
        public int SiteID
        {
            get
            {
                return _siteID;
            }
            set
            {
                this.OnPropertyChanged<int>(ref this._siteID, value, "SiteID");
            }
        }

        /// <summary>
        /// On which carrier the labware mounts, can be empty.
        /// </summary>
        public int CarrierGrid
        {
            get
            {
                return _grid;
            }
            set
            {
                this.OnPropertyChanged<int>(ref this._grid, value, "CarrierGrid");
            }
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
                this.OnPropertyChanged<WellsInfo>(ref this._wellsInfo, value, "WellsInfo");
            }
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
                this.OnPropertyChanged<ZValues>(ref this._zValues, value, "ZValues");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Labware()
        {
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
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Labware>(toXmlFile, this);
        }

        #endregion

        public object Clone()
        {
            Labware copy = new Labware();

            copy._label = "<Need a label>";
            copy._typeName = this._typeName;
            copy._dimension = (Dimension)this.Dimension.Clone();
            copy._backgroundColor = this._backgroundColor;
            copy._grid = this._grid;
            copy._siteID = this._siteID;
            copy._wellsInfo = (WellsInfo)this._wellsInfo.Clone();
            copy._zValues = (ZValues)this._zValues.Clone();

            return copy;
        }
    }

    /// <summary>
    /// four z-heights used in pipetting
    /// </summary>
    public class ZValues : INotifyPropertyChanged, ICloneable
    {
        /// <summary>
        /// PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._zTravel, value, this, "ZTravel", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._zStart, value, this, "ZStart", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._zStart, value, this, "ZStart", this.PropertyChanged);   
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._zMax, value, this, "ZMax", this.PropertyChanged);   
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
    public class WellsInfo : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int         _wellRadius;
        private int         _numberOfWellsX;
        private int         _numberOfWellsY;
        private Point       _firstWellPosition;
        private Point       _lastWellPosition;
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._wellRadius, value, this, "WellRadius", this.PropertyChanged);   
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._numberOfWellsX, value, this, "NumberOfWellsX", this.PropertyChanged);   
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._numberOfWellsY, value, this, "NumberOfWellsY", this.PropertyChanged); 
            }
        }

        /// <summary>
        /// The position of the first well (most top-left well) on labware
        /// </summary>
        public Point FirstWellPosition
        {
            get
            {
                return _firstWellPosition;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Point>(ref this._firstWellPosition, value, this, "FirstWellPosition", this.PropertyChanged); 
            }
        }

        /// <summary>
        /// The position of the last well (most bottom-right well) on labware
        /// </summary>
        public Point LastWellPosition
        {
            get
            {
                return _lastWellPosition;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Point>(ref this._lastWellPosition, value, this, "LastWellPosition", this.PropertyChanged); 
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<BottomShape>(ref this._bottomShape, value, this, "BottomShape", this.PropertyChanged); 
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
        public WellsInfo(Point first, Point last, int xNum, int yNum, BottomShape shape, int r)
        {
            WellRadius = r;
            NumberOfWellsX = xNum;
            NumberOfWellsY = yNum;
            FirstWellPosition = first;
            LastWellPosition = last;
            BottomShape = shape;
        }

        public object Clone()
        {
            return new WellsInfo(this._firstWellPosition, this._lastWellPosition, this._numberOfWellsX, this._numberOfWellsY, this._bottomShape, _wellRadius);
        }
    }
}
