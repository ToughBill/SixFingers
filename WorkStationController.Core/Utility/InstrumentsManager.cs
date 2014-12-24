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
        private string _recpieDirectory = string.Empty;
        private string _liquidClassDirectory = string.Empty;

        // Dictionary<string, T> is for binding, Key (string) is the XML file path of the instrument
        private Dictionary<string, Labware> _labwares = new Dictionary<string, Labware>();
        private Dictionary<string, Carrier> _carriers = new Dictionary<string, Carrier>();
        private Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();
        private Dictionary<string, LiquidClass> _liquidClasses = new Dictionary<string, LiquidClass>();

        // FileSystemWatchers for instrument XML files
        private FileSystemWatcher _fsw_labware = null;
        private FileSystemWatcher _fsw_carrier = null;
        private FileSystemWatcher _fsw_recipe = null;
        private FileSystemWatcher _fsw_liquidclass = null;
        #endregion
        #region events
        public event EventHandler onWareChanged;
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
        /// Gets the existing recipe definitions
        /// </summary>
        public ObservableCollection<Recipe> Recipes
        {
            get
            {
                return new ObservableCollection<Recipe>(this._recipes.Values);
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

            this._recpieDirectory = Path.Combine(currentDirectory, "Recipes");
            if (!Directory.Exists(this._recpieDirectory))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.FolderNotExistsError, this._recpieDirectory));
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
            LoadInstrument<Recipe>(this._recpieDirectory, this._recipes);
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
                try
                {
                    KeyValuePair<string, Labware> lw_kvp = this._labwares.First(kvp => kvp.Value.ID == id);
                    xmlFilePath = lw_kvp.Key;
                    bFound = true;
                }
                catch (Exception)
                {
                    // If no matching, exception thrown and we catch it.
                }
            }
            else if (typeof(T) == typeof(Carrier))
            {
                try
                {
                    KeyValuePair<string, Carrier> ca_kvp = this._carriers.First(kvp => kvp.Value.ID == id);
                    xmlFilePath = ca_kvp.Key;
                    bFound = true;
                }
                catch (Exception)
                {
                    // If no matching, exception thrown and we catch it.
                }
            }
            else if (typeof(T) == typeof(Recipe))
            {
                try
                {
                    KeyValuePair<string, Recipe> lo_kvp = this._recipes.First(kvp => kvp.Value.ID == id);
                    xmlFilePath = lo_kvp.Key;
                    bFound = true;
                }
                catch (Exception)
                {
                    // If no matching, exception thrown and we catch it.
                }
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
                throw new ArgumentOutOfRangeException("T", typeof(T), "T must be Labware/Carrier/Recipe/LiquidClass");
            }
           
            return bFound;
        }

      
        /// <summary>
        /// Save instrument to its proper directory according to its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instrumentElement"></param>
        public void SaveInstrument<T>(T instrumentElement) where T: ISaveName,ISerialization,IGUID
        {
            // Push the in air instrument to InstrumentManager and this instrument will be pop when
            // corresponding XML file created.
            InstrumentsManager.Instance.CreatedInstrument.Push(instrumentElement);
            RemoveExistingOne(instrumentElement);
            string saveName = instrumentElement.SaveName + ".xml";
            string directory = "";
            if (typeof(T) == typeof(Labware))
            {
                FireChangeWareEvent(instrumentElement);
                directory = _labwareDirectory;
            }
            else if (typeof(T) == typeof(Carrier))
            {
                FireChangeWareEvent(instrumentElement);
                directory = _carrierDirectory;
            }   
            else if (typeof(T) == typeof(Recipe))
            {
                directory = _recpieDirectory;
            }
            else if (typeof(T) == typeof(LiquidClass))
            {
                directory = _labwareDirectory;
            }
            else
            {
                throw new ArgumentOutOfRangeException("T", typeof(T), "T must be Labware/Carrier/Recipe/LiquidClass");
            }
            string path = Path.Combine(directory, saveName);
            instrumentElement.Serialize(path);
        }

        private void FireChangeWareEvent(object wareBase)
        {
            if (onWareChanged != null)
                onWareChanged(this, new WareEditArgs((WareBase)wareBase));
        }

        private void RemoveExistingOne<T>(T instrument) where T :  IGUID
        {
            string xmlFilePath;
            if (InstrumentsManager.Instance.FindInstrument<T>(instrument.ID, out xmlFilePath))
            {
                File.Delete(xmlFilePath);
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

            // Recipe XML files watcher
            this._fsw_recipe = new FileSystemWatcher();
            this._fsw_recipe.Path = this._recpieDirectory;
            this._fsw_recipe.Filter = extFilter;
            this._fsw_recipe.Created += OnRecipeXmlFileCreated;
            this._fsw_recipe.Deleted += OnRecipeXmlFileDeleted;

            // LiquidClass XML files watcher
            this._fsw_liquidclass = new FileSystemWatcher();
            this._fsw_liquidclass.Path = this._liquidClassDirectory;
            this._fsw_liquidclass.Filter = extFilter;
            this._fsw_liquidclass.Created += OnLiquidClassXmlFileCreated;
            this._fsw_liquidclass.Deleted += OnLiquidClassXmlFileDeleted;

            // Begin watching
            this._fsw_labware.EnableRaisingEvents = true;
            this._fsw_carrier.EnableRaisingEvents = true;
            this._fsw_recipe.EnableRaisingEvents = true;
            this._fsw_liquidclass.EnableRaisingEvents = true;
        }

        #region FileSystemWatcher event handlers
        private void OnLabwareXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Labware labware = this.CreatedInstrument.Pop() as Labware;
            if (labware == null)
                throw new InvalidOperationException("Labware instance was supposed to be existing.");

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
            Carrier carrier = this.CreatedInstrument.Pop() as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("Carrier instance was supposed to be existing.");

            this._carriers.Add(e.FullPath, carrier);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Carriers"));
        }

        private void OnCarrierXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._carriers.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Carriers"));
        }

        private void OnRecipeXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Recipe recipe = SerializationHelper.Deserialize<Recipe>(e.FullPath);
            this._recipes.Add(e.FullPath, recipe);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Recipes"));
        }

        private void OnRecipeXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._recipes.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Recipes"));
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
        static internal void LoadInstrument<T>(string instrumtsXmlFileDirectory, Dictionary<string, T> instruments) where T : IDeserializationEx
        {
            string[] instrumentXmlFiles = Directory.GetFiles(instrumtsXmlFileDirectory);

            foreach (string instrumentXmlFile in instrumentXmlFiles)
            {
                if (Path.GetExtension(instrumentXmlFile).Equals(".xml"))
                {
                    T instrument = SerializationHelper.Deserialize<T>(instrumentXmlFile);
                    instrument.PostAction();
                    instruments.Add(instrumentXmlFile, instrument);
                }
            }
        }
    }
}
