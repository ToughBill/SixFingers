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
        private int         _siteID = 0;
        private Carrier     _parentCarrier = null;
        private WellsInfo   _wellsInfo = new WellsInfo();
        private ZValues     _zValues = new ZValues();
        protected string    _label = string.Empty;

 

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

        static public Labware CreateFromSkeleton(LabwareSkeleton labwareSkeletonItem, Carrier parentCarrier = null)
        {
            // TODO: Complete member initialization
            List<Labware> labwares = new List<Labware>(InstrumentsManager.Instance.Labwares);
            var baseLabware = labwares.Find(x => x.TypeName == labwareSkeletonItem.TypeName);
            if( baseLabware == null)
                throw new Exception(string.Format("Cannot find the specified labware: ", labwareSkeletonItem.TypeName));
            var newLabware = (Labware)baseLabware.Clone();
            newLabware.Label = labwareSkeletonItem.Label;
            newLabware.SiteID = labwareSkeletonItem.SiteID;
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
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Labware>(toXmlFile, this);
        }

        #endregion

        public object Clone()
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
        /// update the information which is NOT common
        /// </summary>
        /// <param name="templateLabware"></param>
        public void CarryInfo(Labware templateLabware)
        {
            this.Label = templateLabware.Label;
            this._parentCarrier = templateLabware.ParentCarrier;
            this.SiteID = templateLabware.SiteID;
        }

        /// <summary>
        /// post action - nothing
        /// </summary>
        public void PostAction()
        {

        }
    }

    /// <summary>
    /// Four z-heights used in pipetting
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._zDispense, value, this, "ZDispense", this.PropertyChanged);   
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
        private double      _firstWellPositionX;
        private double      _firstWellPositionY;
        private double      _lastWellPositionX;
        private double      _lastWellPositionY;
        private BottomShape _bottomShape;

        private Point       _firstWellPosition;
        private Point       _lastWellPosition;

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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._firstWellPositionX, value, this, "FirstWellPositionX", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._firstWellPositionY, value, this, "FirstWellPositionY", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._lastWellPositionX, value, this, "LastWellPositionX", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._lastWellPositionY, value, this, "LastWellPositionY", this.PropertyChanged);
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
    }
}
