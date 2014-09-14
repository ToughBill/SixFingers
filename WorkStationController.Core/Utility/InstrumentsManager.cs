using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// InstumentsManager is an utility class that load all the instruments(Labware/Carrier/Recipes/LuiquidClass)
    /// when application startup and monitoring the instruments XML file changing.
    /// </summary>
    public class InstrumentsManager
    {
        /// <summary>
        /// Single instance of InstrumentsManager
        /// </summary>
        private InstrumentsManager _singleInstance = null;

        /// <summary>
        /// Gets the single insntace of InstrumentsManager
        /// </summary>
        public InstrumentsManager Instance
        {
            get
            {
                if(this._singleInstance == null)
                {
                    this._singleInstance = new InstrumentsManager();
                }

                return this._singleInstance;
            }
        }

        // ObservableCollection<T> is for binding
        private ObservableCollection<Labware> _labwares = new ObservableCollection<Labware>();
        private ObservableCollection<Carrier> _carriers = new ObservableCollection<Carrier>();
        private ObservableCollection<Layout> _layouts = new ObservableCollection<Layout>();
        private ObservableCollection<LiquidClass> _liquidClasses = new ObservableCollection<LiquidClass>();

        /// <summary>
        /// Gets the existing Labware definitions
        /// </summary>
        public ObservableCollection<Labware> Labwares
        {
            get
            {
                return this._labwares;
            }
        }

        /// <summary>
        /// Gets the existing Carrier definitions
        /// </summary>
        public ObservableCollection<Carrier> Carriers
        {
            get
            {
                return this._carriers;
            }
        }

        /// <summary>
        /// Gets the existing Layout definitions
        /// </summary>
        public ObservableCollection<Layout> Layouts
        {
            get
            {
                return this._layouts;
            }
        }

        /// <summary>
        /// Gets the existing LiquidClass definitions
        /// </summary>
        public ObservableCollection<LiquidClass> LiquidClasses
        {
            get
            {
                return this._liquidClasses;
            }
        }

        // Paths for each instruments folder
        private string _labwareDirectory = string.Empty;
        private string _carrierDirectory = string.Empty;
        private string _layoutDirectory = string.Empty;
        private string _liquidClassDirectory = string.Empty;

        /// <summary>
        /// Internal default constructor
        /// </summary>
        internal InstrumentsManager()
        {
        }

        /// <summary>
        /// Check directory for each instrument and load all instruments from XML files.
        /// </summary>
        public void Initialize()
        {
            #region Check directory for each instrument
            string currentDirectory = Assembly.GetExecutingAssembly().Location;

            // If any of the instrument directory not exists, the initialization process abort.
            this._labwareDirectory = Path.Combine(currentDirectory, "Labwares");
            if(!Directory.Exists(this._labwareDirectory))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.FolderNotExistsError, this._labwareDirectory));
            }

            this._carrierDirectory = Path.Combine(currentDirectory, "Carriers");
            if (!Directory.Exists(this._carrierDirectory))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.FolderNotExistsError, this._carrierDirectory));
            }

            this._layoutDirectory = Path.Combine(currentDirectory, "Recipes");
            if (!Directory.Exists(this._layoutDirectory))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.FolderNotExistsError, this._layoutDirectory));
            }

            this._liquidClassDirectory = Path.Combine(currentDirectory, "LiquidClasses");
            if (!Directory.Exists(this._liquidClassDirectory))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.FolderNotExistsError, this._liquidClassDirectory));
            }
            #endregion

            #region Load each instruments
            LoadInstrument<Labware>(this._labwareDirectory, this._labwares);
            LoadInstrument<Carrier>(this._carrierDirectory, this._carriers);
            LoadInstrument<Layout>(this._layoutDirectory, this._layouts);
            LoadInstrument<LiquidClass>(this._liquidClassDirectory, this._liquidClasses);
            #endregion
        }

        /// <summary>
        /// Generic method for loading instrumts via XML files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instrumtsXmlFileDirectory"></param>
        /// <param name="instruments"></param>
        static internal void LoadInstrument<T>(string instrumtsXmlFileDirectory, ObservableCollection<T> instruments)
        {
            string[] instrumentXmlFiles = Directory.GetFiles(instrumtsXmlFileDirectory);

            foreach (string instrumentXmlFile in instrumentXmlFiles)
            {
                if (Path.GetExtension(instrumentXmlFile).Equals(".xml"))
                {
                    T instrument = SerializationHelper.Deserialize<T>(instrumentXmlFile);
                    instruments.Add(instrument);
                }
            }
        }
    }
}
