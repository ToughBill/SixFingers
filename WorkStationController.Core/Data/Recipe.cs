using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// recipe describes a test of biology
    /// </summary>
    [Serializable]
    public class Recipe : Layout, ISerialization
    {
        /// <summary>
        /// scripts to be executed
        /// </summary>
        [XmlArray("Script text")]
        [XmlArrayItem("script ine", typeof(string), IsNullable = false)]
        public List<string> Scripts { get; set; }

        public void Serialize(string toXmlFile)
        {
            GetSkeletonInfos();
            SerializationHelper.Serialize(toXmlFile, this);
        }

        public Recipe():base()
        {
            Scripts = new List<string>();
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
