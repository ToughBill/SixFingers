using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Carrier installed on worktable
    /// </summary>
    [Serializable]
    public class Carrier : WareBase
    {
        public const int undefinedGrid = 0;
        private List<Labware>                   _labwares = new List<Labware>();                     // Labwares on the carrier
        private ObservableCollection<Site>      _sites = new ObservableCollection<Site>();           // sites for mounting labwares
        private ObservableCollection<string>    _allowedLabwareTypeNames = new ObservableCollection<string>();


        private double _xoffset = default(double);
        private double _yoffset = default(double);
        private int _grid = 0;



        /// <summary>
        /// The X offset of the left-top corner of carrier against the most left-top pin the carrier installed on
        /// </summary>
        [XmlElement]
        public double XOffset
        {
            get { return this._xoffset; }
            set { SetProperty(ref _xoffset, value); }
        }

        /// <summary>
        /// The Y offset of the left-top corner of carrier against the most left-top pin the carrier installed on
        /// </summary>
        [XmlElement]
        public double YOffset
        {
            get { return this._yoffset; }
            set { SetProperty(ref _yoffset, value); }
        }

        /// <summary>
        /// Column id of the plastic pin on which the carrier sits, 1 based
        /// </summary>
        [XmlElement]
        public int GridID
        {
            get { return this._grid; }
            set { SetProperty(ref _grid, value); }
        }

        /// <summary>
        /// Gets the labwares on the carrier
        /// </summary>
        [XmlArray("Labwares")]
        [XmlArrayItem("Labware", typeof(Labware), IsNullable = false)]
        public List<Labware> Labwares
        {
            get
            {
                return this._labwares;
            }
        }

        /// <summary>
        /// carrier can have one or more sites
        /// </summary>
        [XmlArray("Sites")]
        [XmlArrayItem("Site", typeof(Site), IsNullable = false)]
        public ObservableCollection<Site> Sites
        {
            get
            {
                return this._sites;
            }
            set
            {
                _sites = new ObservableCollection<Site>();
            }
        }

        /// <summary>
        /// The labwares that are acceptable.
        /// </summary>
        [XmlArray("AllowedLabwareTypeNames")]
        [XmlArrayItem("AllowedLabwareTypeName", typeof(string), IsNullable = false)]
        public ObservableCollection<string> AllowedLabwareTypeNames
        {
            get
            {
                return _allowedLabwareTypeNames;
            }
            set
            {
                SetProperty(ref _allowedLabwareTypeNames, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Carrier()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buildInType"></param>
        public Carrier(BuildInCarrierType buildInType)
        {
            CreateBuildInCarrier(buildInType);
            
        }

        private void CreateBuildInCarrier(BuildInCarrierType buildInType)
        {
            switch (buildInType)
            {
                case BuildInCarrierType.MP_3POS:
                    CreateMP_3POS();
                    break;
                case BuildInCarrierType.Tube13mm_16POS:
                    CreateTube13mm_16Pos();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carrierTrait"></param>
        /// <returns></returns>
        static public Carrier CreateFromTrait(CarrierTrait carrierTrait)
        {
            List<Carrier> carriers = new List<Carrier>(PipettorElementManager.Instance.Carriers);
            Carrier theCarrier = carriers.Find(x => x.TypeName == carrierTrait.TypeName);
            if (theCarrier == null)
                return null;
                //throw new Exception(string.Format("Cannot find the specified carrier: ", carrierTrait.TypeName));
            Carrier newCarrier = (Carrier)theCarrier.Clone();
            newCarrier.GridID = carrierTrait.GridID;
            return newCarrier;
        }
        
        /// <summary>
        /// Create an instance of Carrier from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Carrier instance</returns>
        public static Carrier Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Carrier>(fromXmlFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labware"></param>
        public void AddLabware(Labware labware)
        {
            if (labware == null)
            {
                throw new ArgumentNullException("labware", "labware must not be null.");
            }

            if (_labwares.Contains(labware))
                return;
            labware.ParentCarrier = this;
            this._labwares.Add(labware);
        }

        /// <summary>
        /// Remove a labware from carrier
        /// </summary>
        /// <param name="labware">Instance of labware to remove</param>
        public void RemoveCarrier(Labware labware)
        {
            if (labware == null)
            {
                throw new ArgumentNullException("carrier", "carrier must not be null.");
            }

            this._labwares.Remove(labware);
        }

        /// <summary>
        /// Remove a labware from carrier by lable
        /// </summary>
        /// <param name="labwareName">Lable of labware to remove</param>
        public void RemoveLabware(string labwareName)
        {
            Labware labware = this._labwares.Find(l => l.TypeName == labwareName);

            if(labware != null)
            {
                this._labwares.Remove(labware);
            }
        }

        #region Serialization

        /// <summary>
        /// Serialize to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for serialization</param>
        public override void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Carrier>(toXmlFile, this);
        }

        #endregion

        public override object Clone()
        {
            Carrier newCarrier = new Carrier();
            newCarrier.TypeName = _typeName;
            newCarrier.BackgroundColor = _backgroundColor;
            newCarrier.XOffset = _xoffset;
            newCarrier.YOffset = _yoffset;
            foreach (string allowedLabwareTypeName in this.AllowedLabwareTypeNames)
                newCarrier.AllowedLabwareTypeNames.Add(allowedLabwareTypeName);
            foreach (Site site in _sites)
                newCarrier.Sites.Add(site.Clone() as Site);
            newCarrier.Dimension = _dimension.Clone() as Dimension;
            newCarrier.GridID = undefinedGrid;
            return newCarrier;
        }

        private void CreateTube13mm_16Pos()
        {
            _dimension = new Data.Dimension(24, 316);
            _xoffset = 120;
            _yoffset = 247;
            Site site1 = new Site(0, 11, 0, 24, 305, 1);
            _sites.Add(site1);
            _grid = undefinedGrid;
            _allowedLabwareTypeNames.Add(LabwareBuildInType.Tubes16Pos13_100MM.ToString());
            TypeName = BuildInCarrierType.Tube13mm_16POS.ToString();
        }

        // Will be replaced by xml
        private void CreateMP_3POS() 
        {            
            _dimension = new Data.Dimension(1490, 3160);
            _xoffset = 12;
            _yoffset = 24.7;
            Site site1 = new Site(5.5, 25, 0, 127, 85.5, 1);
            Site site2 = new Site(5.5, 121, 0, 127, 85.5, 2);
            Site site3 = new Site(5.5, 217, 0, 127, 85.5, 3);
            _sites.Add(site1);
            _sites.Add(site2);
            _sites.Add(site3);
            _allowedLabwareTypeNames.Add(LabwareBuildInType.Plate96_05ML.ToString());
            _allowedLabwareTypeNames.Add(LabwareBuildInType.Plate24_2ML.ToString());
            _grid = undefinedGrid;
            TypeName = BuildInCarrierType.MP_3POS.ToString();
        }


        public void CarryInfo(Carrier exampleCarrier)
        {
            if (exampleCarrier == this)
                return;
            _typeName = exampleCarrier.TypeName;
            _backgroundColor = exampleCarrier._backgroundColor;
            XOffset = exampleCarrier.XOffset;
            YOffset = exampleCarrier.YOffset;
            _allowedLabwareTypeNames.Clear();
            foreach (string allowedLabwareTypeName in exampleCarrier.AllowedLabwareTypeNames)
                _allowedLabwareTypeNames.Add(allowedLabwareTypeName);
            _sites.Clear();
            foreach (Site site in exampleCarrier._sites)
                _sites.Add(site.Clone() as Site);
            _dimension = _dimension.Clone() as Dimension;
        }
    }

    /// <summary>
    /// Carrier build-in types
    /// </summary>
    public enum BuildInCarrierType
    {
        MP_3POS = 0,
        Tube13mm_16POS = 1,
        WashStation = 2
    }

    /// <summary>
    /// The definition of labware on carrier position information
    /// </summary>
    [Serializable]
    public class Site : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private double _xoffset = 0.0;
        private double _yoffset = 0.0;
        private double _zoffset = 0.0;
        private double _xsize   = 0.0;
        private double _ysize   = 0.0;
        private int   _id       = -1;       
       
        /// <summary>
        /// Gets or sets the X-Offset of carrier
        /// </summary>
        public double XOffset
        {
            get
            {
                return _xoffset;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._xoffset, value, this, "XOffset", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the Y-Offset of carrier
        /// </summary>
        public double YOffset
        {
            get
            {
                return _yoffset;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._yoffset, value, this, "YOffset", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the Z-Offset of carrier
        /// </summary>
        public double ZOffset
        {
            get
            {
                return _zoffset;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._zoffset, value, this, "ZOffset", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the X-Size of carrier
        /// </summary>
        public double XSize
        {
            get
            {
                return _xsize;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._xsize, value, this, "XSize", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the X-Size of carrier
        /// </summary>
        public double YSize
        {
            get
            {
                return _ysize;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._ysize, value, this, "YSize", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the Id of carrier, 1-based
        /// </summary>
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }   

        /// <summary>
        /// Default constructor
        /// </summary>
        public Site()
        { }

        public Site(double xoffset, double yoffset, double zoffset,
                    double xsize, double ysize, 
                    int id)
        {
            _xoffset = xoffset;
            _yoffset = yoffset;
            _zoffset = zoffset;
            _xsize = xsize;
            _ysize = ysize;
            _id = id;            
        }

        public object Clone()
        {
            return new Site(_xoffset, _yoffset, _zoffset, _xsize, _ysize, _id);
        }
    }
}
