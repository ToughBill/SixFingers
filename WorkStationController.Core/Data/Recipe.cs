using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// recipe describes a test of biology
    /// </summary>
    public class Recipe : Layout, ISerialization
    {
        /// <summary>
        /// scripts to be executed
        /// </summary>
        public List<string> Scripts { get; set; }




        void ISerialization.Serialize(string toXmlFile)
        {
            throw new NotImplementedException();
        }
    }
}
