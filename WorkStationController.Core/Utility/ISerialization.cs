namespace WorkStationController.Core.Utility
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
}
