using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of liquid class
    /// </summary>
    [Serializable]
    public class LiquidClass : ISerialization
    {
        /// <summary>
        /// Aspiration pipetting instance
        /// </summary>
        private AspirationPipetting aspirationSinglePipetting = new AspirationPipetting();

        /// <summary>
        /// Dispense pipetting instance
        /// </summary>
        private DispensePipetting dispenseSinglePipetting = new DispensePipetting();

        /// <summary>
        /// Gets or sets the name of liquid class
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets the aspiration single pipetting parameters
        /// </summary>
        [XmlElement]
        public AspirationPipetting AspirationSinglePipetting 
        { 
            get { return this.aspirationSinglePipetting; }
            set 
            {
                this.aspirationSinglePipetting = null;
                this.aspirationSinglePipetting = value; 
            }
        }

        /// <summary>
        /// Gets the dispense single pipetting parameters
        /// </summary>
        [XmlElement]
        public DispensePipetting DispenseSinglePipetting 
        {
            get { return this.dispenseSinglePipetting; }
            set 
            {
                this.dispenseSinglePipetting = null;
                this.dispenseSinglePipetting = value; 
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LiquidClass()
        { }

        /// <summary>
        /// Create an instance of LiquidClass from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A LiquidClass instance</returns>
        public static LiquidClass Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<LiquidClass>(fromXmlFile);
        }

        /// <summary>
        /// Serialize instance of LiquidClass to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file name</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<LiquidClass>(toXmlFile, this);
        }
    }

    /// <summary>
    /// Aspiration pipetting parameters
    /// </summary>
    [Serializable]
    public class AspirationPipetting : ISerialization
    {
        /// <summary>
        /// Gets or sets the aspiration speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int AspirationSpeed { get; set; }

        /// <summary>
        /// Gets or sets the delay time in unit of ms
        /// </summary>
        [XmlElement]
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets the system trailing airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int SystemTrailingAirgap { get; set; }

        /// <summary>
        /// Gets or sets the leading airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int LeadingAirgap { get; set; }

        /// <summary>
        /// Gets or sets the leading airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int TrailingAirgap { get; set; }

        /// <summary>
        /// Gets or sets the excess volume in unit of μl
        /// </summary>
        [XmlElement]
        public int ExcessVolume { get; set; }

        /// <summary>
        /// Gets or sets the conditioning volume in unit of μl
        /// </summary>
        [XmlElement]
        public int ConditioningVolume { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AspirationPipetting()
        { }

        /// <summary>
        /// Create an instance of AspirationPipetting from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A AspirationPipetting instance</returns>
        public static AspirationPipetting Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<AspirationPipetting>(fromXmlFile);
        }

        /// <summary>
        /// Serialize instance of AspirationPipetting to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file name</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<AspirationPipetting>(toXmlFile, this);
        }
    }

    /// <summary>
    /// Dispense pipetting parameters
    /// </summary>
    [Serializable]
    public class DispensePipetting : ISerialization
    {
        /// <summary>
        /// Gets or sets the dispense speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int DispenseSpeed { get; set; }

        /// <summary>
        /// Gets or sets the delay time in unit of ms
        /// </summary>
        [XmlElement]
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets whether trailing airgap after each dispense
        /// </summary>
        [XmlElement]
        public bool TrailingAirgapAfterDispense { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DispensePipetting()
        { }

        /// <summary>
        /// Create an instance of DispensePipetting from a XML file
        /// </summary>
        /// <param name="fromXmlFile">XML file name</param>
        /// <returns>A DispensePipetting instance</returns>
        public static DispensePipetting Create(string fromXmlFile)
        {
            return SerializationHelper.Deserialize<DispensePipetting>(fromXmlFile);
        }

        /// <summary>
        /// Serialize instance of DispensePipetting to XML file
        /// </summary>
        /// <param name="toXmlFile">XML file name</param>
        public void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<DispensePipetting>(toXmlFile, this);
        }
    }
}
