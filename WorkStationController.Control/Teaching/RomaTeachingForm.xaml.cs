using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for RomaTeachingForm.xaml
    /// </summary>
    public partial class RomaTeachingForm : Window
    {
        PlateVectorCollection plateVectorCollection = new PlateVectorCollection();
        public RomaTeachingForm()
        {
            InitializeComponent();
            LoadPlateVectors();
        }


       
        private void LoadPlateVectors()
        {
            
            string folder = FolderHelper.GetVectorFolder();
            plateVectorCollection.PlateVectors.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            var files = dirInfo.EnumerateFiles("*.xml").ToList();
            foreach(var file in files)
            {
                plateVectorCollection.PlateVectors.Add(SerializationHelper.Deserialize<PlateVector>(file.FullName));
            }
            this.DataContext = plateVectorCollection;
        }

        private void SaveDefineVectors_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveDefineVectors_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(plateVectorCollection.SelectedPlateVector != null)
            {
                SavePlateVector(plateVectorCollection.SelectedPlateVector);
            }
        }

        private void SavePlateVector(PlateVector plateVector)
        {
            string name = plateVector.Name;
            string sFile = FolderHelper.GetVectorFolder() + name + ".xml";
            if(File.Exists(sFile))
            {
                File.Delete(sFile);
            }
            SerializationHelper.Serialize(sFile,plateVector);
            //SetInfo(string.Format("向量已经成功保存到{0}", sFile),false);
        }

        //private void SetInfo(string hint, bool isError = true)
        //{
        //    txtInfo.Text = hint;
        //    Brush txtBrush = isError ? Brushes.Red : Brushes.Black;
        //    txtInfo.Foreground = txtBrush;
        //}

        

        private void btnAddPlateVector_Click(object sender, RoutedEventArgs e)
        {
            var names = plateVectorCollection.PlateVectors.Select(x => x.Name).ToList();
            string vectorName = GetNextName(names);
            PlateVector plateVector = new PlateVector(vectorName);
            plateVectorCollection.Add(plateVector);
        }

        private void btnRemovePlateVector_Click(object sender, RoutedEventArgs e)
        {
            
            
        }


        private void btnAddPosition_Click(object sender, RoutedEventArgs e)
        {
            if (plateVectorCollection.SelectedPlateVector == null)
                return;
            plateVectorCollection.SelectedPlateVector.AddNewPosition();

        }

        private void btnDeletePosition_Click(object sender, RoutedEventArgs e)
        {
            if (plateVectorCollection.SelectedPlateVector == null)
                return;
            if (plateVectorCollection.SelectedPlateVector.Positions == null || plateVectorCollection.SelectedPlateVector.Positions.Count == 0)
                return;
            if (plateVectorCollection.SelectedPlateVector.CurrentPosition != null)
                plateVectorCollection.SelectedPlateVector.RemoveCurrent();
        }


       

        static public string GetExeParentFolder()
        {
            string s = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int index = s.LastIndexOf("\\");
            return s.Substring(0, index) + "\\";
        }

        static public string GetNextName(List<string> existingNames)
        {
            if (existingNames == null || existingNames.Count == 0)
                return "unknown1";
            int id = 1;
            while (true)
            {

                string testName = string.Format("unknown{0}", id);
                if (!existingNames.Contains(testName))
                    return testName;
                id++;
            }
        }
    }
}
