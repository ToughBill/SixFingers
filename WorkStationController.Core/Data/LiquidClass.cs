using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WorkstationController.Core.Utility;

namespace WorkstationController.Core.Data
{
    /// <summary>
    /// Data definition of liquid class
    /// </summary>
    [Serializable]
    public class LiquidClass : 
        ISerialization, 
        INotifyPropertyChanged, 
        ICloneable,
        IDeserializationEx
    {
        #region private members for properties
        private string _typename = "<Need a name>";      
        private AspirationPipetting _aspirationSinglePipetting = new AspirationPipetting();
        private DispensePipetting _dispenseSinglePipetting = new DispensePipetting();
        #endregion

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Unique ID of the LiquidClass
        /// </summary>
        [XmlElement]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of liquid class
        /// </summary>
        [XmlAttribute]
        public string TypeName 
        {
            get { return this._typename; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<string>(ref this._typename, value, this, "Name", this.PropertyChanged); }
        }

        /// <summary>
        /// Gets the aspiration single pipetting parameters
        /// </summary>
        [XmlElement]
        public AspirationPipetting AspirationSinglePipetting 
        {
            get { return this._aspirationSinglePipetting; }
            set 
            {
                this._aspirationSinglePipetting = null;
                this._aspirationSinglePipetting = value;
                PropertyChangedNotifyHelper.NotifyPropertyChanged<AspirationPipetting>(ref this._aspirationSinglePipetting, value, this, "AspirationSinglePipetting", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets the dispense single pipetting parameters
        /// </summary>
        [XmlElement]
        public DispensePipetting DispenseSinglePipetting 
        {
            get { return this._dispenseSinglePipetting; }
            set 
            {
                this._dispenseSinglePipetting = null;
                this._dispenseSinglePipetting = value;
                PropertyChangedNotifyHelper.NotifyPropertyChanged<DispensePipetting>(ref this._dispenseSinglePipetting, value, this, "DispenseSinglePipetting", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LiquidClass()
        {
            this.ID = Guid.NewGuid();
        }

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

        /// <summary>
        /// Clone a new instance of AspirationPipetting with same value as current instance
        /// </summary>
        /// <returns>New instance of AspirationPipetting with same value as current instance</returns>
        public object Clone()
        {
            LiquidClass copy = new LiquidClass();

            copy._aspirationSinglePipetting = (AspirationPipetting)this._aspirationSinglePipetting.Clone();
            copy._dispenseSinglePipetting = (DispensePipetting)this._dispenseSinglePipetting.Clone();

            return copy;
        }



        /// <summary>
        /// post action - nothing
        /// </summary>
        public void PostAction()
        {

        }
    }

    /// <summary>
    /// Aspiration pipetting parameters
    /// </summary>
    [Serializable]
    public class AspirationPipetting : ISerialization, INotifyPropertyChanged, ICloneable
    {
        #region private members for properties
        private int _aspirationSpeed = 0;
        private int _delay = 0;
        private int _systemTrailingAirgap = 0;
        private int _leadingAirgap = 0;
        private int _trailingAirgap = 0;
        private int _excessVolume = 0;
        private int _conditioningVolume = 0;
        #endregion

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Gets or sets the aspiration speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int AspirationSpeed 
        {
            get { return this._aspirationSpeed; }
            set { PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._aspirationSpeed, value, this, "AspirationSpeed", this.PropertyChanged); }
        }

        /// <summary>
        /// Gets or sets the delay time in unit of ms
        /// </summary>
        [XmlElement]
        public int Delay 
        {
            get { return this._delay; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._delay, value, this, "Delay", this.PropertyChanged);
            } 
        }

        /// <summary>
        /// Gets or sets the system trailing airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int SystemTrailingAirgap
        {
            get { return this._systemTrailingAirgap; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._systemTrailingAirgap, value, this, "SystemTrailingAirgap", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the leading airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int LeadingAirgap
        {
            get { return this._leadingAirgap; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._leadingAirgap, value, this, "LeadingAirgap", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the leading airgap in unit of μl
        /// </summary>
        [XmlElement]
        public int TrailingAirgap
        {
            get { return this._trailingAirgap; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._trailingAirgap, value, this, "TrailingAirgap", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the excess volume in unit of μl
        /// </summary>
        [XmlElement]
        public int ExcessVolume
        {
            get { return this._excessVolume; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._excessVolume, value, this, "ExcessVolume", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the conditioning volume in unit of μl
        /// </summary>
        [XmlElement]
        public int ConditioningVolume
        {
            get { return this._conditioningVolume; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._conditioningVolume, value, this, "ConditioningVolume", this.PropertyChanged);
            }
        }

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

        /// <summary>
        /// Clone a new instance of AspirationPipetting with same value as current instance
        /// </summary>
        /// <returns>New instance of AspirationPipetting with same value as current instance</returns>
        public object Clone()
        {
            AspirationPipetting copy = new AspirationPipetting();

            copy._aspirationSpeed = this._aspirationSpeed;
            copy._delay = this._delay;
            copy._systemTrailingAirgap = this._systemTrailingAirgap;
            copy._leadingAirgap = this._leadingAirgap;
            copy._trailingAirgap = this._trailingAirgap;
            copy._excessVolume = this._excessVolume;
            copy._conditioningVolume = this._conditioningVolume;

            return copy;
        }
    }

    /// <summary>
    /// Dispense pipetting parameters
    /// </summary>
    [Serializable]
    public class DispensePipetting : ISerialization, INotifyPropertyChanged, ICloneable
    {
        #region private members for properties
        private int _dispenseSpeed = 0;
        private int _delay = 0;
        private bool _trailingAirgapAfterDispense = false;
        #endregion

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Gets or sets the dispense speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int DispenseSpeed
        {
            get { return this._dispenseSpeed; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._dispenseSpeed, value, this, "DispenseSpeed", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets the delay time in unit of ms
        /// </summary>
        [XmlElement]
        public int Delay
        {
            get { return this._delay; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<int>(ref this._delay, value, this, "Delay", this.PropertyChanged);
            }
        }

        /// <summary>
        /// Gets or sets whether trailing airgap after each dispense
        /// </summary>
        [XmlElement]
        public bool TrailingAirgapAfterDispense
        {
            get { return this._trailingAirgapAfterDispense; }
            set
            {
                PropertyChangedNotifyHelper.NotifyPropertyChanged<bool>(ref this._trailingAirgapAfterDispense, value, this, "TrailingAirgapAfterDispense", this.PropertyChanged);
            }
        }

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

        /// <summary>
        /// Clone a new instance of DispensePipetting with same value as current instance
        /// </summary>
        /// <returns>New instance of DispensePipetting with same value as current instance</returns>
        public object Clone()
        {
            DispensePipetting copy = new DispensePipetting();

            copy._dispenseSpeed = this._dispenseSpeed;
            copy._delay = this._delay;
            copy._trailingAirgapAfterDispense = this._trailingAirgapAfterDispense;

            return copy;
        }
    }
}
