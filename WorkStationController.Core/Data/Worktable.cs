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
        /// Distance between adjacent pins in mm.
        /// </summary>
        public const int DistanceBetweenAdjacentPins = 25;

        /// <summary>
        /// The size of the worktable
        /// </summary>
        public Size Size{ get; set; }

        /// <summary>
        /// The number of grid
        /// </summary>
        public int GridCount{get; set;}

        // The size of each rows' pin, normally the first row's size is smaller
        // but the second & third are same.

        /// <summary>
        /// first row pin, a little smaller
        /// </summary>
        public Size PinSize{ get; set; }

        /// <summary>
        /// Position of the left-top pin
        /// </summary>
        public Point TopLeftPinPosition{ get; set; }

        /// <summary>
        /// The pin's Y position of the second row, there are 3 rows of pin
        /// </summary>
        public int BottomLeftPinYPosition{ get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Worktable()
        { }

        /// <summary>
        /// Create an instance of Worktable
        /// </summary>
        public Worktable(Size sz,
            Size pinSize,
            Point topLeftPinPos,
            int bottomLeftPintYPos,
            int gridCount)
        {
            Size = sz;
            PinSize = pinSize;
            TopLeftPinPosition = topLeftPinPos;
            BottomLeftPinYPosition = bottomLeftPintYPos;
            this.GridCount = gridCount;
        }

        /// <summary>
        /// Create an instance of Worktable from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>An instance of Worktable</returns>
        public static Worktable Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Worktable>(fromXmlFile);
        }

        /// <summary>
        /// Serialize an instance of Worktable to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file name</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Worktable>(toXmlFile, this);
        }
    }
}
