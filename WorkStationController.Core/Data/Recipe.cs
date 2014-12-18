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
    public class Recipe : Layout, ISerialization, IDeserializationEx, INotifyPropertyChanged
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
            GetSkeletonInfos();
            SerializationHelper.Serialize(toXmlFile, this);
        }

        /// <summary>
        /// UID of the recipe
        /// </summary>
        [XmlAttribute]
        public Guid ID { get; set; }

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
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public Recipe():base()
        {
            this.ID = Guid.NewGuid();
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

        /// <summary>
        /// post action - nothing
        /// </summary>
        public void PostAction()
        {
            _carriers = RestoreCarriersFromSkeleton(_carrierSkeletons, _labwareSkeletons);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
