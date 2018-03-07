using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WorkstationController.Core.Properties;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// recipe describes a test of biology
    /// </summary>
    [Serializable]
    public class Recipe : Layout
    {
        private string _recipeName;
        /// <summary>
        /// scripts to be executed
        /// </summary>
        [XmlArray("Script text")]
        [XmlArrayItem("script ine", typeof(string), IsNullable = false)]
        public List<string> Scripts { get; set; }

  
        public Recipe():base()
        {
            _recipeName = Resources.LabelNotDefined;
            Scripts = new List<string>();
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
        /// Create an instance of Layout from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A Layout instance</returns>
        public static Recipe Create(string fromXmlFile)
        {
            Recipe recipe = SerializationHelper.Deserialize<Recipe>(fromXmlFile);
            recipe._carriers = RestoreCarriersFromTrait(recipe._carrierTraits, recipe._labwareTraits);
            //ConstrainTipInfo(recipe);
            return recipe;
        }

       
       

        #region override base

        /// <summary>
        /// serialize
        /// </summary>
        /// <param name="toXmlFile"></param>
       

        public override string TypeName
        {
            get
            {
                return "Recipe";
            }
        }
        
        /// <summary>
        /// the name would be used in saveing and loading
        /// </summary>
        [XmlIgnoreAttribute] 
        public override string SaveName
        {
            get
            {
                return Name;
            }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

       
        #endregion
    }
}
