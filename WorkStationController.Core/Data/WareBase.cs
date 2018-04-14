using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Carrier|Labware's common base
    /// </summary>
    [Serializable]
    public abstract class WareBase : PipettorElement,ICloneable
    {
        protected string    _typeName = "<Need a name>";
        protected Dimension _dimension = new Dimension();
        protected Color _backgroundColor = Colors.Gray;

        /// <summary>
        /// Gets or sets the typeName of the ware
        /// </summary>
        public override string TypeName
        {
            get{ return _typeName; }
            set { SetProperty(ref _typeName, value); }
        }
        /// <summary>
        /// Background color
        /// </summary>
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        [XmlIgnore]
        public override string SaveName
        {
            get
            {
                return _typeName;
            }
            set
            {
                TypeName = value;
            }
        }
        

        /// <summary>
        /// Gets or sets the width and height of the ware
        /// </summary>
        public Dimension Dimension
        {
            get{ return _dimension; }
            set { SetProperty(ref _dimension, value); }
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
    public class Dimension : BindableBase, ICloneable
    {
        private double _xLength = 0;
        private double _yLength = 0;
        /// <summary>
        /// Gets or sets the X-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        public double XLength
        {
            get
            {
                return _xLength;
            }
            set
            {
                SetProperty(ref _xLength, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y-length of the labware, in 1/10 millimetre(mm.)
        /// </summary>
        public double YLength
        {
            get
            {
                return _yLength;
            }
            set
            {
                SetProperty(ref _yLength, value);
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
        public Dimension(double x, double y)
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
