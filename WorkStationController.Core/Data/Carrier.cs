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
    public class Carrier : ISerialization, INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Private Members
        private List<Labware> _labwares = new List<Labware>();  // Labwares on the carrier
        private List<Site> _sites = new List<Site>(); // sites for mounting labwares

        private string _name = string.Empty;
        private int _xlength = default(int);
        private int _ylength = default(int);
        private int _xoffset = default(int);
        private int _yoffset = default(int);
        private int _grid = 0;
        #endregion

        [XmlElement]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lable of the carrier
        /// </summary>
        [XmlAttribute]
        public string Name 
        {
            get { return this._name; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<string>(ref this._name, value, this, "Name", this.PropertyChanged); }
        }


        /// <summary>
        /// Gets or sets the X-length of the carrier, in 0.1 millimetre(0.1 mm.)
        /// </summary>
        [XmlAttribute]
        public int XLength 
        {
            get { return this._xlength; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._xlength, value, this, "XLength", this.PropertyChanged); } 
        }

        /// <summary>
        /// Gets or sets the Y-length of the carrier, in 0.1 millimetre(0.1 mm.)
        /// </summary>
        [XmlAttribute]
        public int YLength
        {
            get { return this._ylength; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._ylength, value, this, "YLength", this.PropertyChanged); }
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
            Labware labware = this._labwares.Find(l => l.Name == labwareName);

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

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The definition of labware on carrier position information
    /// </summary>
    [Serializable]
    public class Site
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public List<LabwareType> AllowedLabwareTypes { get; set; }
    }

    //[Serializable]
    //public class LabwarePositionOnCarrier : INotifyPropertyChanged
    //{
    //    #region Private members
    //    private int _site = 0;
    //    private double _xoffset = default(double);
    //    private double _yoffset = default(double);
    //    private double _zoffset = default(double);
    //    private double _xsize = default(double);
    //    private double _ysize = default(double);
    //    #endregion

    //    // Notify property changed event handler
    //    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    //    /// <summary>
    //    /// Gets or sets the Site of labware on carrier
    //    /// </summary>
    //    public int Site
    //    {
    //        get { return this._site; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._site, value, this, "Site", this.PropertyChanged); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the X-Offiset of labware on carrier
    //    /// </summary>
    //    public double XOffset
    //    {
    //        get { return this._xoffset; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._xoffset, value, this, "XOffset", this.PropertyChanged); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the Y-Offiset of labware on carrier
    //    /// </summary>
    //    public double YOffset
    //    {
    //        get { return this._yoffset; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._yoffset, value, this, "YOffset", this.PropertyChanged); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the Z-Offiset of labware on carrier
    //    /// </summary>
    //    public double ZOffset
    //    {
    //        get { return this._zoffset; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._zoffset, value, this, "ZOffset", this.PropertyChanged); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the X-Size of labware on carrier
    //    /// </summary>
    //    public double XSize
    //    {
    //        get { return this._xsize; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._xsize, value, this, "XSize", this.PropertyChanged); }
    //    }
    //    /// <summary>
    //    /// Gets or sets the Y-Size of labware on carrier
    //    /// </summary>
    //    public double YSize
    //    {
    //        get { return this._ysize; }
    //        set { PropertyChangedNotifyHelper.NotifyPropertyChanged<double>(ref this._ysize, value, this, "YSize", this.PropertyChanged); }
    //    }
    //}
}
