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
    /// Data definition of a labware installed on the carrier
    /// </summary>
    [Serializable]
    public class Labware : WareBase,ISerialization
    {
        
        private Color       _backGroundColor;
        private int         _siteID;
        private LabwareType _labwareType;
        private string      _carrierLabel;

        
        /// <summary>
        /// Default constructor
        /// </summary>
        public Labware()
        {
        }

        /// <summary>
        /// labware type, there can be several labwares with same typename in one layout
        /// </summary>
        public LabwareType LabwareType
        {
            get
            {
                return _labwareType;
            }
            set
            {
                _labwareType = value;
            }
        }

        /// <summary>
        /// back ground color
        /// </summary>
        public Color BackGroundColor
        {
            get
            {
                return _backGroundColor;
            }
            set
            {
                _backGroundColor = value;
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
                _siteID = value;
            }

        }

        /// <summary>
        /// on which carrier the labware mounts, can be empty.
        /// </summary>
        public string CarrierLabel
        {
            get
            {
                return _carrierLabel;
            }
            set
            {
                _carrierLabel = value;
            }
        }

        private WellsInfo _wellsInfo;
        private Dimension _dimension;
        private ZValues _zValues;

        /// <summary>
        /// the info of the wells on the ware
        /// </summary>
        public WellsInfo WellsInfo
        {
            get
            {
                return _wellsInfo;
            }
            set
            {
                _wellsInfo = value;
            }
        }

        /// <summary>
        /// the width and height
        /// </summary>
        public Dimension Dimension
        {
            get
            {
                return _dimension;
            }
            set
            {
                _dimension = value;
            }
        }
        /// <summary>
        /// liquid class related
        /// </summary>
        public ZValues ZValues
        {
            get
            {
                return _zValues;
            }
            set
            {
                _zValues = value;
            }
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
    }



    /// <summary>
    /// labware's type
    /// </summary>
    public enum LabwareType
    {
        /// <summary>
        /// diti
        /// </summary>
        Tips = 0,

        /// <summary>
        /// plate
        /// </summary>
        Microplates = 1,

        /// <summary>
        /// wash
        /// </summary>
        Wash = 2,

        /// <summary>
        /// waste
        /// </summary>
        Waste = 3,

        /// <summary>
        /// normally a rectangle container
        /// </summary>
        Trough = 4,

        /// <summary>
        /// tube is widely used in hospital
        /// </summary>
        Tubes = 5,

        /// <summary>
        /// misc wares
        /// </summary>
        Misc = 6,
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
    /// see before
    /// </summary>
    public class Dimension : INotifyPropertyChanged
    {
        private int _xLength;
        private int _yLength;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// make the xml serializer happy
        /// </summary>
        public Dimension()
        {

        }
        /// <summary>
        /// Gets or sets the X-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        public int XLength
        {
            get
            {
                return _xLength;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                    (ref this._xLength, value, this, "XLength", this.PropertyChanged);
                
            }
        }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        public int YLength 
        { 
            get
            {
                return _yLength;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                    (ref this._yLength, value, this, "YLength", this.PropertyChanged);
            }
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Dimension(int x, int y)
        {
            XLength = x;
            YLength = y;
        }
    }

    /// <summary>
    /// four z-heights used in pipetting
    /// </summary>
    public class ZValues : INotifyPropertyChanged
    {
        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int _zTravel;
        private int _zStart;
        public int _zMax;
        private int _zDispense;

        /// <summary>
        /// make the serializer happy
        /// </summary>
        public ZValues()
        {

        }

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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                    (ref this._zTravel, value, this, "ZTravel", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                    (ref this._zStart, value, this, "ZStart", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Dispense value, in 1/10 millimetre
        /// </summary>
        public int ZDispense
        {
            get
            {
                return _zDispense;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                       (ref this._zStart, value, this, "ZStart", this.PropertyChanged);   
            }
        }

        /// <summary>
        /// Gets or sets the Z-Max value, in 1/10 millimetre
        /// </summary>
        public int ZMax
        {
            get
            {
                return _zMax;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                       (ref this._zMax, value, this, "ZMax", this.PropertyChanged);   
            }
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
    }

    /// <summary>
    /// the well information on the labware
    /// </summary>
    public class WellsInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int _wellRadius;
        private int _numberOfWellsX;
        private int _numberOfWellsY;
        private Point _firstWellPosition;
        private Point _lastWellPosition;
        private BottomShape _bottomShape;

        /// <summary>
        /// make the xml serializer happy
        /// </summary>
        public WellsInfo()
        {

        }
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                       (ref this._wellRadius, value, this, "WellRadius", this.PropertyChanged);   
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                       (ref this._numberOfWellsX, value, this, "NumberOfWellsX", this.PropertyChanged);   
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>
                       (ref this._numberOfWellsY, value, this, "NumberOfWellsY", this.PropertyChanged); 
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Point>
                      (ref this._firstWellPosition, value, this, "FirstWellPosition", this.PropertyChanged); 
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Point>
                      (ref this._lastWellPosition, value, this, "LastWellPosition", this.PropertyChanged); 
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<BottomShape>
                     (ref this._bottomShape, value, this, "BottomShape", this.PropertyChanged); 
            }
        }

        /// <summary>
        /// ctor
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
    }
}
