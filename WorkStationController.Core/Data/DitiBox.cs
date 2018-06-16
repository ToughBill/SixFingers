using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Core.Data
{
    public class DitiBox
    {
        public static DitiType Parse(string sTypeName)
        {
            if (!sTypeName.Contains("Diti"))
                sTypeName = "Diti" + sTypeName;

            Dictionary<string, DitiType> typeName_DitiType = new Dictionary<string, DitiType>();
            typeName_DitiType.Add("Diti1000", DitiType.OneK);
            typeName_DitiType.Add("Diti200", DitiType.TwoHundred);
            typeName_DitiType.Add("Diti50", DitiType.Fifty);
            typeName_DitiType.Add("Diti10", DitiType.Ten);
            
            return typeName_DitiType[sTypeName];
        }
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
