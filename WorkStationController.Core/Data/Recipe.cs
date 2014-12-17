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
            GetSkeletonInfos();
            SerializationHelper.Serialize<Layout>(toXmlFile, this);
        }

        /// <summary>
        /// Create an instance of Layout from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Layout instance</returns>
        public static Recipe Create(string fromXmlFile)
        {
            Recipe recipe = SerializationHelper.Deserialize<Recipe>(fromXmlFile);
            recipe._carriers = RestoreCarriersFromSkeleton(recipe._carrierSkeletons, recipe._labwareSkeletons);
            return recipe;
        }
    }
}
