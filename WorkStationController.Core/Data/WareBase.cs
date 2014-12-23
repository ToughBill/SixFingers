using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Carrier|Labware's common base
    /// </summary>
    [Serializable]
    public abstract class WareBase : 
        INotifyPropertyChanged,
        ISaveName,
        IGUID
    {
        protected Guid      _id = Guid.Empty;
        protected string    _typeName = "<Need a name>";
        protected string    _label = string.Empty;
        protected Dimension _dimension = new Dimension();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        // Utility property changed notify method
        protected void OnPropertyChanged<T>(ref T oldValue, T setValue, string propertyName)
        {
            PropertyChangedNotifyHelper.NotifyPropertyChanged<T>(ref oldValue, setValue, this, propertyName, this.PropertyChanged);
        }

        /// <summary>
        /// UID of the ware
        /// </summary>
        public Guid ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// Gets or sets the typeName of the ware
        /// </summary>
        public string TypeName
        {
            get{ return _typeName; }
            set{ PropertyChangedNotifyHelper.NotifyPropertyChanged<string>(ref this._typeName, value, this, "TypeName", this.PropertyChanged); }
        }

        /// <summary>
        /// the name would be used in saveing & loading
        /// </summary>
        [XmlIgnoreAttribute] 
        public string SaveName
        {
            get
            {
                return TypeName;
            }
            set
            {
                TypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the label of the ware
        /// </summary>
        public string Label
        {
            get { return this._label; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<string>(ref this._label, value, this, "Label", this.PropertyChanged); }
        }

        /// <summary>
        /// Gets or sets the width and height of the ware
        /// </summary>
        public Dimension Dimension
        {
            get{ return _dimension; }
            set{ PropertyChangedNotifyHelper.NotifyPropertyChanged<Dimension>(ref this._dimension, value, this, "Dimension", this.PropertyChanged); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WareBase()
        {
            this._id = Guid.NewGuid();
        }

        /// <summary>
        /// make binding happy
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._typeName;
        }
    }

    /// <summary>
    /// see before
    /// </summary>
    public class Dimension : INotifyPropertyChanged, ICloneable
    {
        private int _xLength = 0;
        private int _yLength = 0;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._xLength, value, this, "XLength", this.PropertyChanged);
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
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._yLength, value, this, "YLength", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Dimension()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Dimension(int x, int y)
        {
            this._xLength = x;
            this._yLength = y;
        }

        /// <summary>
        /// as name means
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Dimension(this._xLength, this._yLength);
        }
    }
}
