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

        /// <summary>
        /// Serialize object to an XML node
        /// </summary>
        /// <param name="toXmlNode">XML node for saving object</param>
        void Seialize(XmlNode toXmlNode);
    }
}
