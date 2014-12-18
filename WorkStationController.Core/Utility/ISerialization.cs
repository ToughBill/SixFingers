using System.Xml;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// Interface for serialization
    /// </summary>
    interface ISerialization
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
    interface IDeserializationEx
    {
        void PostAction();
    }

}
