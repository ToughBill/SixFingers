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
    public class LiquidClass : PipettorElement
    {
        #region private members for properties
        private string _typename = "<Need a name>";      
        private AspirationPipetting _aspirationSinglePipetting = new AspirationPipetting();
        private DispensePipetting _dispenseSinglePipetting = new DispensePipetting();
        private DispensePipetting _dispenseMultiPipetting = new DispensePipetting();
        #endregion

        /// <summary>
        /// Gets or sets the name of liquid class
        /// </summary>
        [XmlElement]
        public override string TypeName 
        {
            get { return this._typename; }
            set { SetProperty(ref _typename, value); }
        }

        /// <summary>
        /// Gets the dispense multiple pipetting parameters
        /// </summary>
        [XmlElement]
        public DispensePipetting DispenseMultiPipetting
        {
            get { return this._dispenseMultiPipetting; }
            set
            {
                SetProperty(ref _dispenseMultiPipetting, value);
            }
        }

        public override string SaveName
        {
            get
            {
                return this._typename;
            }
            set
            {
                this._typename = value;
            }
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
                SetProperty(ref _aspirationSinglePipetting, value);
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
                SetProperty(ref _dispenseSinglePipetting, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LiquidClass()
        {
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
        public override void Serialize(string toXmlFile)
        {
            SerializationHelper.Serialize<LiquidClass>(toXmlFile, this);
        }

        /// <summary>
        /// Clone a new instance of AspirationPipetting with same value as current instance
        /// </summary>
        /// <returns>New instance of AspirationPipetting with same value as current instance</returns>
        public override object Clone()
        {
            LiquidClass copy = new LiquidClass();
            copy._aspirationSinglePipetting = (AspirationPipetting)this._aspirationSinglePipetting.Clone();
            copy._dispenseSinglePipetting = (DispensePipetting)this._dispenseSinglePipetting.Clone();
            return copy;
        }
    }

    /// <summary>
    /// Aspiration pipetting parameters
    /// </summary>
    [Serializable]
    public class AspirationPipetting : BindableBase, ISerialization, ICloneable
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
        /// Gets or sets the aspiration speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int AspirationSpeed 
        {
            get { return this._aspirationSpeed; }
            set { SetProperty(ref _aspirationSpeed, value); }
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
                SetProperty(ref _delay, value);
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
                SetProperty(ref _systemTrailingAirgap, value);
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
                SetProperty(ref _leadingAirgap, value);
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
                SetProperty(ref _trailingAirgap, value);                
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
                SetProperty(ref _excessVolume, value);                
            }
        }

        ///// <summary>
        ///// Gets or sets the conditioning volume in unit of μl
        ///// </summary>
        //[XmlElement]
        //public int ConditioningVolume
        //{
        //    get { return this._conditioningVolume; }
        //    set
        //    {
        //        SetProperty(ref _conditioningVolume, value);
        //    }
        //}

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
    public class DispensePipetting :BindableBase, ISerialization, ICloneable
    {
        #region private members for properties
        private int _dispenseSpeed = 0;
        private int _delay = 0;
        private bool _trailingAirgapAfterDispense = false;
        #endregion

        /// <summary>
        /// Gets or sets the dispense speed in unit of μl / s
        /// </summary>
        [XmlElement]
        public int DispenseSpeed
        {
            get { return this._dispenseSpeed; }
            set
            {
                SetProperty(ref _dispenseSpeed, value);
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
                SetProperty(ref _delay, value);
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
                SetProperty(ref _trailingAirgapAfterDispense, value);
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
