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
    /// InstumentsManager is an utility class that load all the pipettorElements(Labware/Carrier/Recipes/LuiquidClass)
    /// when application startup and monitoring the pipettorElements XML file changing.
    /// </summary>
    public sealed class PipettorElementManager : INotifyPropertyChanged
    {
        #region Private members
        // Single instance of pipettorElementsManager
        private static PipettorElementManager _singleInstance = null;

        // Newly created pipettorElement have to be added to this stack
        private Stack<object> _newlyCreatedPipettorElement = new Stack<object>();

        // Paths for each pipettorElements folder
        private string _labwareDirectory = string.Empty;
        private string _carrierDirectory = string.Empty;
        private string _layoutDirectory = string.Empty;
        private string _liquidClassDirectory = string.Empty;

        // Dictionary<string, T> is for binding, Key (string) is the XML file path of the pipettorElement
        private Dictionary<string, Labware> _labwares = new Dictionary<string, Labware>();
        private Dictionary<string, Carrier> _carriers = new Dictionary<string, Carrier>();
        private Dictionary<string, Layout> _layouts = new Dictionary<string, Layout>();
        private Dictionary<string, LiquidClass> _liquidClasses = new Dictionary<string, LiquidClass>();

        // FileSystemWatchers for pipettorElement XML files
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
        /// Gets the single insntace of pipettorElementsManager
        /// </summary>
        public static PipettorElementManager Instance
        {
            get
            {
                if(_singleInstance == null)
                {
                    _singleInstance = new PipettorElementManager();
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
        /// Gets the existing layout definitions
        /// </summary>
        public ObservableCollection<Layout> Layouts
        {
            get
            {
                return new ObservableCollection<Layout>(this._layouts.Values);
            }
        }


        /// <summary>
        /// Newly created pipettorElement have to be added to this stack
        /// </summary>
        public Stack<object> CreatedPipettorElement
        {
            get { return this._newlyCreatedPipettorElement; }
        }

        /// <summary>
        /// Internal default constructor
        /// </summary>
        internal PipettorElementManager()
        {
        }

        /// <summary>
        /// Check directory for each pipettorElement and load all pipettorElements from XML files.
        /// </summary>
        public void Initialize()
        {
            #region Check directory for each pipettorElement
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // If any of the pipettorElement directory not exists, the initialization process abort.
            this._labwareDirectory = Path.Combine(currentDirectory, "Labwares");
            if(!Directory.Exists(this._labwareDirectory))
            {
                Directory.CreateDirectory(_labwareDirectory);
            }

            this._carrierDirectory = Path.Combine(currentDirectory, "Carriers");
            if (!Directory.Exists(this._carrierDirectory))
            {
                Directory.CreateDirectory(_carrierDirectory);
            }

            this._layoutDirectory = Path.Combine(currentDirectory, "Layouts");
            if (!Directory.Exists(this._layoutDirectory))
            {
                Directory.CreateDirectory(_layoutDirectory);
            }

            this._liquidClassDirectory = Path.Combine(currentDirectory, "LiquidClasses");
            if (!Directory.Exists(this._liquidClassDirectory))
            {
                Directory.CreateDirectory(_liquidClassDirectory);
            }
            #endregion

            #region Load each pipettorElements
            LoadPipettorElements<Labware>(this._labwareDirectory, this._labwares);
            LoadPipettorElements<Carrier>(this._carrierDirectory, this._carriers);
            LoadPipettorElements<Layout>(this._layoutDirectory, this._layouts);
            LoadPipettorElements<LiquidClass>(this._liquidClassDirectory, this._liquidClasses);
            #endregion

            this.InitalizeFileSystemWatches();
        }

        /// <summary>
        /// Check if an instance a type of pipettor had been serialized.
        /// </summary>
        /// <typeparam name="T">Type of the pipettorElement</typeparam>
        /// <param name="elementName">name of the element instance</param>
        /// <param name="xmlFilePath">The XML file path of the pipettorElement</param>
        /// <returns>If the pipettorElement had been serialized, return true, otherwise return false</returns>
        public bool FindPipettorElement<T>(string elementName, out string xmlFilePath)
        {
            bool bFound = false;
            xmlFilePath = string.Empty;
            
            if(typeof(T) == typeof(Labware))
            {
                try
                {
                    KeyValuePair<string, Labware> lw_kvp = this._labwares.First(kvp => kvp.Value.TypeName == elementName);
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
                    KeyValuePair<string, Carrier> ca_kvp = this._carriers.First(kvp => kvp.Value.TypeName == elementName);
                    xmlFilePath = ca_kvp.Key;
                    bFound = true;
                }
                catch (Exception)
                {
                    // If no matching, exception thrown and we catch it.
                }
            }
            else if (typeof(T) == typeof(Layout))
            {
                try
                {
                    KeyValuePair<string, Layout> lo_kvp = this._layouts.First(kvp => kvp.Value.SaveName == elementName);
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
                    KeyValuePair<string, LiquidClass> lc_kvp = this._liquidClasses.First(kvp => kvp.Value.TypeName == elementName);
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
        /// Save pipettorElement to its proper directory according to its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pipettorElement"></param>
        public void SavePipettorElement<T>(T pipettorElement) where T: PipettorElement
        {
            // Push the in air pipettorElement to pipettorElementManager and this pipettorElement will be pop when
            // corresponding XML file created.
            if (pipettorElement.SaveName ==null || pipettorElement.SaveName == "" || pipettorElement.SaveName.Contains("<") || pipettorElement.SaveName.Contains(">"))
                throw new Exception("名称非法！");
            PipettorElementManager.Instance.CreatedPipettorElement.Push(pipettorElement);
            RemoveExistingOne(pipettorElement);
            string saveName = pipettorElement.SaveName + ".xml";
            string directory = "";
            bool need2Update = false;
            if (typeof(T) == typeof(Labware))
            {
                need2Update = true;
                directory = _labwareDirectory;
            }
            else if (typeof(T) == typeof(Carrier))
            {
                need2Update = true;
                directory = _carrierDirectory;
            }   
            else if (typeof(T) == typeof(Layout))
            {
                directory = _layoutDirectory;
            }
            else if (typeof(T) == typeof(LiquidClass))
            {
                directory = _liquidClassDirectory;
            }
            else
            {
                throw new ArgumentOutOfRangeException("T", typeof(T), "T must be Labware/Carrier/Layout/LiquidClass");
            }
            string path = Path.Combine(directory, saveName);
            pipettorElement.Serialize(path);
            if(need2Update)
                FireChangeWareEvent(pipettorElement);
        }

        private void FireChangeWareEvent(object wareBase)
        {
            if (onWareChanged != null)
                onWareChanged(this, new WareEditArgs((WareBase)wareBase));
        }

        private void RemoveExistingOne<T>(T pipettorElement) where T : PipettorElement
        {
            string xmlFilePath;
            if (PipettorElementManager.Instance.FindPipettorElement<T>(pipettorElement.TypeName, out xmlFilePath))
            {
                File.Delete(xmlFilePath);
            }
        }

        /// <summary>
        /// Delete an pipettorElement by its type and ID
        /// </summary>
        /// <typeparam name="T">The type of the pipettorElement</typeparam>
        /// <param name="typeName">The ID of the pipettorElement</param>
        public void DeletePipettorElement<T>(string typeName)
        {
           string xmlFilePath = string.Empty;
           if(FindPipettorElement<T>(typeName, out xmlFilePath))
           {
               File.Delete(xmlFilePath);
           }
        }

        /// <summary>
        /// Initialize the file system watches for pipettorElements XML files.
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
            this._fsw_recipe.Path = this._layoutDirectory;
            this._fsw_recipe.Filter = extFilter;
            this._fsw_recipe.Created += OnLayoutXmlFileCreated;
            this._fsw_recipe.Deleted += OnnLayoutXmlFileDeleted;

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
            Labware labware = this.CreatedPipettorElement.Pop() as Labware;
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
            Carrier carrier = this.CreatedPipettorElement.Pop() as Carrier;
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

        private void OnLayoutXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            Layout layout = SerializationHelper.Deserialize<Layout>(e.FullPath);
            this._layouts.Add(e.FullPath, layout);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Layouts"));
        }

        private void OnnLayoutXmlFileDeleted(object sender, FileSystemEventArgs e)
        {
            this._layouts.Remove(e.FullPath);
            this.PropertyChanged(this, new PropertyChangedEventArgs("Layouts"));
        }

        private void OnLiquidClassXmlFileCreated(object sender, FileSystemEventArgs e)
        {
            LiquidClass liquidClass = this.CreatedPipettorElement.Pop() as LiquidClass;
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
        /// Generic method for loading pipettor elements via XML files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementsXmlFileDirectory"></param>
        /// <param name="pipettorElements"></param>
        static internal void LoadPipettorElements<T>(string elementsXmlFileDirectory, Dictionary<string, T> pipettorElements) where T : PipettorElement
        {
            string[] pipettorElementXmlFiles = Directory.GetFiles(elementsXmlFileDirectory);

            foreach (string pipettorElementXmlFile in pipettorElementXmlFiles)
            {
                if (Path.GetExtension(pipettorElementXmlFile).Equals(".xml"))
                {
                    T element = SerializationHelper.Deserialize<T>(pipettorElementXmlFile);
                    element.DoExtraWork();
                    pipettorElements.Add(pipettorElementXmlFile,element);
                }
            }
        }
    }
}
