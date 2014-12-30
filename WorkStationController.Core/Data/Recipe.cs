using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class Recipe : Layout,ISerialization, ICloneable
    {
        private string _recipeName;
        /// <summary>
        /// scripts to be executed
        /// </summary>
        [XmlArray("Script text")]
        [XmlArrayItem("script ine", typeof(string), IsNullable = false)]
        public List<string> Scripts { get; set; }

        public void Serialize(string toXmlFile)
        {
            GetTraitsInfo();
            SerializationHelper.Serialize(toXmlFile, this);
        }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return _recipeName;
            }
            set
            {
                _recipeName = value;
                SetProperty(ref _recipeName, value);
            }
        }


        /// <summary>
        /// the name would be used in saveing & loading
        /// </summary>
        [XmlIgnoreAttribute] 
        public string SaveName
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
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
            recipe._carriers = RestoreCarriersFromTrait(recipe._carrierTraits, recipe._labwareTraits);
            return recipe;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
