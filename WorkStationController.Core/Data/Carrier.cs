using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Carrier installed on worktable
    /// </summary>
    [Serializable]
    public class Carrier :WareBase, ISerialization, INotifyPropertyChanged, ICloneable
    {
        public const int undefinedGrid = 0;
        /// <summary>
        /// nothing to say
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged = delegate { };
        #region Private Members
        private List<Labware> _labwares = new List<Labware>();  // Labwares on the carrier
        private List<Site> _sites = new List<Site>(); // sites for mounting labwares
        
        private string _name = string.Empty;
        private int _xoffset = default(int);
        private int _yoffset = default(int);
        private int _grid = 0;
        
        #endregion

        /// <summary>
        /// guid, do we really need this?
        /// </summary>
        [XmlElement]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// The X offset of the left-top corner of carrier against the most left-top pin the carrier installed on
        /// </summary>
        [XmlElement]
        public int XOffset
        {
            get { return this._xoffset; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._xoffset, value, this, "XOffset", this.PropertyChanged); }
        }

        /// <summary>
        /// The Y offset of the left-top corner of carrier against the most left-top pin the carrier installed on
        /// </summary>
        [XmlElement]
        public int YOffset
        {
            get { return this._yoffset; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._yoffset, value, this, "YOffset", this.PropertyChanged); }
        }

        /// <summary>
        /// Column id of the plastic pin on which the carrier sits
        /// </summary>
        [XmlElement]
        public int Grid
        {
            get { return this._grid; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._grid, value, this, "Grid", this.PropertyChanged); }
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
        [XmlArrayItem("Labware", typeof(Site), IsNullable = false)]
        public List<Site> Sites
        {
            get
            {
                return this._sites;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public Carrier()
        {
            this.ID = Guid.NewGuid();
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
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Carrier>(toXmlFile, this);
        }

        #endregion

        /// <summary>
        /// as name mean
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public Carrier(BuildInCarrierType buildinType)
        {
            switch(buildinType)
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

        private void CreateTube13mm_16Pos()
        {
            //_xlength = 240;
            //_ylength = 3160;
            
            _xoffset = 120;
            _yoffset = 247;
            Site site1 = new Site(new Point(0, 110), new Size(240, 3050), new List<string> { LabwareBuildInType.Tubes16Pos13_100MM.ToString() });
             _sites.Add(site1);
            _grid = undefinedGrid;
            TypeName = BuildInCarrierType.Tube13mm_16POS.ToString();
        }

        private void CreateMP_3POS() //will be replaced by xml
        {
            
            //_xlength = 1490;
            //_ylength = 3160;
            _xoffset = 120;
            _yoffset = 247;
            Site site1 = new Site(new Point(55, 250), new Size(1270, 850), new List<string> { LabwareBuildInType.Plate96_05ML.ToString()});
            Site site2 = new Site(new Point(55, 1210), new Size(1270, 850), new List<string> { LabwareBuildInType.Plate96_05ML.ToString() });
            Site site3 = new Site(new Point(55, 2170), new Size(1270, 850), new List<string> { LabwareBuildInType.Plate96_05ML.ToString() });
            _sites.Add(site1);
            _sites.Add(site2);
            _sites.Add(site3);

            _grid = undefinedGrid;
            TypeName = BuildInCarrierType.MP_3POS.ToString();
        }
    }

    /// <summary>
    /// carrier builded in types
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
    public class Site : INotifyPropertyChanged
    {
        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private Point _position;
        private Size _sz;
        private ObservableCollection<string> _allowedLabwareTypeNames;
        
        public Site(Point position, Size sz, List<string> allowedLabwareTypeNames)
        {
            _position = position;
            _sz = sz;
            _allowedLabwareTypeNames = new ObservableCollection<string>(allowedLabwareTypeNames);
        }
        /// <summary>
        /// position
        /// </summary>
        public Point Position 
        {
            get
            {
                return _position;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Point>
                    (ref this._position, value, this, "Position", this.PropertyChanged);
            }
        }


   
        /// <summary>
        /// size
        /// </summary>
        public Size Size 
        { 
            get
            {
                return _sz;
            }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<Size>
                    (ref this._sz, value, this, "Size", this.PropertyChanged);
            }
        }
        /// <summary>
        /// the labwares that are acceptable.
        /// </summary>
        public ObservableCollection<string> AllowedLabwareTypeNames
        {
            get
            {
                return _allowedLabwareTypeNames;
            }
            set
            {
                _allowedLabwareTypeNames = value;
            }
        }
    }
}
