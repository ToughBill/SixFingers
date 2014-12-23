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
        /// Serialize object to an XML file
        /// </summary>
        /// <param name="toXmlFile">XML file for saving object</param>
        void Serialize(string toXmlFile);
        
    }
    /// <summary>
    /// do extra work after standard deserialization
    /// </summary>
    public interface IDeserializationEx
    {
        void PostAction();
    }

    /// <summary>
    /// the name which used in saving & loading
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
