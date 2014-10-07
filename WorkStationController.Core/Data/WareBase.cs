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
        private string _typeName;
        private string _label;
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
        /// make binding happy
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _typeName;
        }
    }
}
