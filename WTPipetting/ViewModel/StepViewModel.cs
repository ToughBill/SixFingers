using WTPipetting.Utility;
using WTPipetting.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WTPipetting
{
    class StepViewModel : ViewModelBase
    {
        ObservableCollection<StepDesc> stepDescs = new ObservableCollection<StepDesc>();
        public StepViewModel()
        {
            string sDataFolder =  FolderHelper.GetImageFolder();
            BitmapImage sampleDef = new BitmapImage(new Uri(sDataFolder + "sampleDef.png"));
            BitmapImage barcodeDef = new BitmapImage(new Uri(sDataFolder + "barcodeDef.jpg"));
            BitmapImage genScript = new BitmapImage(new Uri(sDataFolder + "genScript.jpg"));

            stepDescs.Add(new StepDesc("样品定义", sampleDef, Stage.SampleDef));
            stepDescs.Add(new StepDesc("条码设置", barcodeDef, Stage.BarcodeDef));
            stepDescs.Add(new StepDesc("运行实验", genScript, Stage.StepMonitor));
        }

        public ObservableCollection<StepDesc> StepsModel
        {
            get
            {
                return stepDescs;
            }
            set
            {
                stepDescs = value;
            }
        }
    }


    public enum Stage
    {
        SampleDef = 0,
        BarcodeDef = 1,
        StepMonitor = 2
    };

    class StepDesc
    {
        string name;
        Stage correspondingStage;
        BitmapImage image;

        public StepDesc(string name, BitmapImage bmp, Stage stage)
        {
            this.name = name;
            this.image = bmp;
            correspondingStage = stage;
        }
        public Stage CorrespondingStage
        {
            get { return correspondingStage; }
            set { correspondingStage = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public BitmapImage Image
        {
            get { return image; }
            set { image = value; }
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            StepDesc anotherDesc = obj as StepDesc;
            if ((System.Object)anotherDesc == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (name == anotherDesc.name);
        }

        public static bool operator ==(StepDesc a, StepDesc b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(StepDesc a, StepDesc b)
        {
            return !(a == b);
        }

    }
}
