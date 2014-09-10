using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.Core.Utility;

namespace WorkStationController.Core.Data
{
     public class Worktable : ISerialization
    {
        public Size worktableSize;
        
         // the size of each rows' pin, normally the first row's size is smaller
        // but the second & third are same.
        public Size firstRowPinSize;
        public Size secondRowPinSize;
        public Size thirdRowPinSize; 

        //position of the left/top pin
        public Point firstPinPosition;

        //the pin's y position of the second row, there are 3 rows of pin
        public double yPosRows2Pin;
        public double yPosRows3Pin;

        private Worktable()
        {
        }

        //pin's color is defaulted to black
        //worktable's color is default to lightGray
        //public void Draw(System.Windows.Media.DrawingContext drawingContext, Size wholeWindow)
        //{
        //    throw new NotImplementedException();
        //}
        public static Worktable Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<Worktable>(fromXmlFile);
        }

        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<Worktable>(toXmlFile, this);
        }
    }
}
