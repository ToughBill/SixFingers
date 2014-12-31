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

    /// <summary>
    /// the name which used in saving and loading
    /// </summary>
    public interface ISaveName
    {
        string SaveName { get; }
    }

    /// <summary>
    /// guid
    /// </summary>
    public interface IGUID
    {
        Guid ID { get; set; }
    }

    /// <summary>
    /// do extra work
    /// </summary>
    public interface  IDeserializationEx
    {
       void DoExtraWork();
    }

}
