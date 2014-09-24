using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using WorkstationController.Core.Data;

namespace WorkstationController.Core.Utility
{
    /// <summary>
    /// InstumentsManager is an utility class that load all the instruments(Labware/Carrier/Recipes/LuiquidClass)
    /// when application startup and monitoring the instruments XML file changing.
    /// </summary>
    public sealed class InstrumentsManager : INotifyPropertyChanged
    {
        #region Private members
        // Single instance of InstrumentsManager
        private static InstrumentsManager _singleInstance = null;

        // Newly created instrument have to be added to this stack
        private Stack<object> _newlyCreatedInstrument = new Stack<object>();

        // Paths for each instruments folder
        private string _labwareDirectory = string.Empty;
        private string _carrierDirectory = string.Empty;
        private string _layoutDirectory = string.Empty;
        private string _liquidClassDirectory = string.Empty;

        // Dictionary<string, T> is for binding, Key (string) is the XML file path of the instrument
        private Dictionary<string, Labware> _labwares = new Dictionary<string, Labware>();
        private Dictionary<string, Carrier> _carriers = new Dictionary<string, Carrier>();
        private Dictionary<string, Layout> _layouts = new Dictionary<string, Layout>();
        private Dictionary<string, LiquidClass> _liquidClasses = new Dictionary<string, LiquidClass>();

        // FileSystemWatchers for instrument XML files
        private FileSystemWatcher _fsw_labware = null;
        private FileSystemWatcher _fsw_carrier = null;
        private FileSystemWatcher _fsw_layout = null;
        private FileSystemWatcher _fsw_liquidclass = null;
        #endregion

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Gets the single insntace of InstrumentsManager
        /// </summary>
        public static InstrumentsManager Instance
        {
            get
            {
                if(_singleInstance == null)
                {
                    _singleInstance = new InstrumentsManager();
                }

                return _singleInstance;
            }
        }

        /// <summary>
        /// Gets the existing Labware definitions
        /// </summary>
        public ObservableCollection<Labware> Labwares
        {
            get
            {
                return new ObservableCollection<Labware>(this._labwares.Values);
            }
        }

        /// <summary>
        /// Gets the existing Carrier definitions
        /// </summary>
        public ObservableCollection<Carrier> Carriers
        {
            get
            {
                return new ObservableCollection<Carrier>(this._carriers.Values);
            }
        }

        /// <summary>
        /// Gets the existing Layout definitions
        /// </summary>
        public ObservableCollection<Layout> Layouts
        {
            get
            {
                return new ObservableCollection<Layout>(this._layouts.Values);
            }
        }

        /// <summary>
        /// Gets the existing LiquidClass definitions
        /// </summary>
        public ObservableCollection<LiquidClass> LiquidClasses
        {
            get
            {
                return new ObservableCollection<LiquidClass>(this._liquidClasses.Values);
            }
        }

        /// <summary>
        /// Newly created instrument have to be added to this stack
        /// </summary>
        public Stack<object> CreatedInstrument
        {
            get { return this._newlyCreatedInstrument; }
        }

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
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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

            this.InitalizeFileSystemWatches();
        }

        /// <summary>
        /// Check if an instance a type of instrument had been serialized.
        /// </summary>
        /// <typeparam name="T">Type of the instrument</typeparam>
        /// <param name="id">ID of the instrument instance</param>
        /// <param name="xmlFilePath">The XML file path of the instrument</param>
        /// <returns>If the instrument had been serialized, return true, otherwise return false</returns>
        public bool FindInstrument<T>(Guid id, out string xmlFilePath)
        {
            if (id == null || id == Guid.Empty)
                throw new ArgumentException("id could not be null of empty GUID", "id");

            bool bFound = false;
            xmlFilePath = string.Empty;

            if(typeof(T) == typeof(Labware))
            {

            }
            else if (typeof(T) == typeof(Carrier))
            {

            }
            else if (typeof(T) == typeof(Layout))
            {

            }
            else if (typeof(T) == typeof(LiquidClass))
            {
                try
                {
                    KeyValuePair<string, LiquidClass> lc_kvp = this._liquidClasses.First(kvp => kvp.Value.ID == id);
                    xmlFilePath = lc_kvp.Key;
                    bFound = true;
                }
                catch(Exception)
                {
                    // If no matching, exception thrown and we catch it.
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("T", typeof(T), "T must be Labware/Carrier/Layout/LiquidClass");
            }
           

            return bFound;
        }

        /// <summary>
        /// Save instrument to its proper directory according to its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instrument"></param>
        public void SaveInstrument<T>(T instrument)
        {
            if (typeof(T) == typeof(Labware))
            {

            }
            else if (typeof(T) == typeof(Carrier))
            {

            }
            else if (typeof(T) == typeof(Layout))
            {

            }
            else if (typeof(T) == typeof(LiquidClass))
            {
                LiquidClass liquidClass = instrument as LiquidClass;
                string path = Path.Combine(this._liquidClassDirectory, liquidClass.ID.ToString() + ".xml");
                liquidClass.Serialize(path);
            }
            else
            {
                throw new ArgumentOutOfRangeException("T", typeof(T), "T must be Labware/Carrier/Layout/LiquidClass");
            }
        }

        /// <summary>
        /// Delete an instrument by its type and ID
        /// </summary>
        /// <typeparam name="T">The type of the instrument</typeparam>
        /// <param name="id">The ID of the instrument</param>
        public void DeleteInstrument<T>(Guid id)
        {
           string xmlFilePath = string.Empty;
           if(FindInstrument<T>(id, out xmlFilePath))
           {
               File.Delete(xmlFilePath);
           }
        }

        /// <summary>
        /// Initialize the file system watches for instruments XML files.
        /// </summary>
        internal void InitalizeFileSystemWatches()
        {
            string extFilter = "*.xml";

            // Labware XML files watcher
            this._fsw_labware = new FileSystemWatcher();
            this._fsw_labware.Path = this._labwareDirectory;
            this._fsw_labware.Filter = extFilter;
            this._fsw_labware.Created += OnLabwareXmlFileCreated;
            this._fsw_labware.Deleted += OnLabwareXmlFileDeleted;

            // Carrier XML files watcher
            this._fsw_carrier = new FileSystemWatcher();
            this._fsw_carrier.Path = this._carrierDirectory;
            this._fsw_carrier.Filter = extFilter;
            this._fsw_carrier.Created += OnCarrierXmlFileCreated;
            this._fsw_carrier.Deleted += OnCarrierXmlFileDeleted;

            // Layout XML files watcher
            this._fsw_layout = new FileSystemWatcher();
            this._fsw_layout.Path = this._layoutDirectory;
            this._fsw_layout.Filter = extFilter;
            this._fsw_layout.Created += OnLayoutXmlFileCreated;
            this._fsw_layout.Deleted += OnLayoutXmlFileDeleted;

            // LiquidClass XML files watcher
            this._fsw_liquidclass = new FileSystemWatcher();
            this._fsw_liquidclass.Path = this._liquidClassDirectory;
            this._fsw_liquidclass.Filter = extFilter;
            this._fsw_liquidclass.Created += OnLiquidClassXmlFileCreated;
            this._fsw_liquidclass.Deleted += OnLiquidClassXmlFileDeleted;

            // Begin watching
            this._fsw_labware.EnableRaisingEvents = true;
            this._fsw_carrier.EnableRaisingEvents = true;
            this._fsw_layout.EnableRaisingEvents = true;
            this._fsw_liquidclass.EnableRaisingEvents = true;
        }

        #region FileSystemWatcher event handlers
        private void OnLabwareXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Labware labware = SerializationHelper.Deserialize<Labware>(e.FullPath);
            this._labwares.Add(e.FullPath, labware);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Labwares"));
        }

        private void OnLabwareXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._labwares.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Labwares"));
        }

        private void OnCarrierXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Carrier carrier = SerializationHelper.Deserialize<Carrier>(e.FullPath);
            this._carriers.Add(e.FullPath, carrier);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Carriers"));
        }

        private void OnCarrierXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._carriers.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Carriers"));
        }

        private void OnLayoutXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Layout layout = SerializationHelper.Deserialize<Layout>(e.FullPath);
            this._layouts.Add(e.FullPath, layout);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Layouts"));
        }

        private void OnLayoutXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._layouts.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Layouts"));
        }

        private void OnLiquidClassXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            LiquidClass liquidClass = this.CreatedInstrument.Pop() as LiquidClass;
            if (liquidClass == null)
                throw new InvalidOperationException("LiquidClass instance was supposed to be existing.");

            this._liquidClasses.Add(e.FullPath, liquidClass);
            this.PropertyChanged(this, new PropertyChangedEventArgs("LiquidClasses"));
        }

        private void OnLiquidClassXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._liquidClasses.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("LiquidClasses"));
        }
        #endregion

        /// <summary>
        /// Generic method for loading instrumts via XML files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instrumtsXmlFileDirectory"></param>
        /// <param name="instruments"></param>
        static internal void LoadInstrument<T>(string instrumtsXmlFileDirectory, Dictionary<string, T> instruments)
        {
            string[] instrumentXmlFiles = Directory.GetFiles(instrumtsXmlFileDirectory);

            foreach (string instrumentXmlFile in instrumentXmlFiles)
            {
                if (Path.GetExtension(instrumentXmlFile).Equals(".xml"))
                {
                    T instrument = SerializationHelper.Deserialize<T>(instrumentXmlFile);
                    
                    instruments.Add(instrumentXmlFile, instrument);
                }
            }
        }
    }
}
