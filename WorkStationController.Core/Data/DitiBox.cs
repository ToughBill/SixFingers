using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class DitiBox : Labware
    {
        public const string typeName = "DitiBox";
        /// <summary> 
        /// if there are several tips on the Liha, for example 4 tips,  we
        /// will fetch tips from front to rear, unless we are going to fetch only one
        /// then, we fetch the tip from rear to front.
        /// </summary>
        public int FrontStartPosition { get; set; }
        public int RearStartPosition { get; set; }
        public new string TypeName { get { return typeName; } }
        public DitiType DitiType { get; set; }
    }
    
    /// <summary>
    /// from 1000ul to 10ul
    /// </summary>
    public enum DitiType
    {
        /// <summary>
        /// // 1000
        /// </summary>
        OneK = 0,   

        /// <summary>
        /// 200
        /// </summary>
        TwoHundred, 

        /// <summary>
        /// 50
        /// </summary>
        Fifty,      

        /// <summary>
        /// 10
        /// </summary>
        Ten,        
    }

}
