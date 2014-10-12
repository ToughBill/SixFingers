using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// carrier|labware's common base
    /// </summary>
    public class WareBase
    {
        protected string _typeName;
        protected string _label;
        protected Dimension _dimension;

        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Gets or sets the typeName of the labware
        /// </summary>
        public string TypeName
        {
            get
            {
                return _typeName;
            }
            set
            {

                _typeName = value;
                OnPropertyChanged("TypeName");
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
                OnPropertyChanged("Dimension");
            }
        }

   
        /// <summary>
        /// make binding happy
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _typeName;
        }
    }

    /// <summary>
    /// see before
    /// </summary>
    public class Dimension : INotifyPropertyChanged
    {
        private int _xLength;
        private int _yLength;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

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
                _xLength = value;
                OnPropertyChanged("XLength");

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
                _yLength = value;
                OnPropertyChanged("YLength");
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
}
