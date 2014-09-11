using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of Worktable
    /// </summary>
    [Serializable]
    public class Worktable : ISerialization
    {
        /// <summary>
        /// Distance between adjancent pins, in unit of mm
        /// </summary>
        public const double distanceMMBetweenAdjacentPins = 25.0;

        /// <summary>
        /// Gets or sets the size of the worktable
        /// </summary>
        public Size Size{ get; set; }
        
        //
        // The size of each rows' pin, normally the first row's size is smaller
        // but the second & third are same.
        //

        /// <summary>
        /// The size of pin in first row
        /// </summary>
        public Size FirstRowPinSize{ get; set; }

        /// <summary>
        /// The size of pin in second row
        /// </summary>
        public Size SecondRowPinSize{ get; set; }

        /// <summary>
        /// The size of pin in thrid row
        /// </summary>
        public Size ThirdRowPinSize{ get; set; } 

        /// <summary>
        /// Position of the most left-top pin
        /// </summary>
        public Point FirstPinPosition{ get; set; }

        //
        // The pin's y position of the second row, there are 3 rows of pin
        //

        /// <summary>
        /// The Y position of the second row
        /// </summary>
        public double YPosRows2Pin{ get; set; }

        /// <summary>
        /// The Y position of the third row
        /// </summary>
        public double YPosRows3Pin{ get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        internal Worktable()
        {
        }

        /// <summary>
        /// Create an instance of Worktable from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Worktable instance</returns>
        public static Worktable Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Worktable>(fromXmlFile);
        }

        /// <summary>
        /// Serialize to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for serialization</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Worktable>(toXmlFile, this);
        }
    }
}
