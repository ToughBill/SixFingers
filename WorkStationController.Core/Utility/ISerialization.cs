using System;
using System.Xml;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// Interface for serialization
    /// </summary>
    public interface ISerialization
    {
        /// <summary>
        /// Serialize object to a XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for saving object</param>
        void Serialize(string toXmlFile);
    }

    public interface IDeserialization
    {
        /// <summary>
        /// deserialize object from a XML file
        /// </summary>
        /// <param name="fromXmlFile"></param>
        T Deserialize<T>(string fromXmlFile);
    }
    /// <summary>
    /// the name which used in saving and loading
    /// </summary>
    public interface ISaveName
    {
        string SaveName { get; set; }
    }

    /// <summary>
    /// guid
    /// </summary>
    public interface IGUID
    {
        Guid ID { get; set; }
    }

}
