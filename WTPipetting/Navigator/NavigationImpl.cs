using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WTPipetting.Data;
using WTPipetting.Interfaces;

namespace WTPipetting.Navigation
{
    public class StageArgs : EventArgs
    {
        protected Stage _stage;
        public StageArgs(Stage stage)
        {
            _stage = stage;
        }
    }

    public class Navigate2Args : StageArgs
    {
        public Stage DestStage
        {
            get
            {
                return _stage;
            }
        }
        
        public Navigate2Args(Stage stage2Go):base(stage2Go)
        {

        }
    }

    public class StageFinishedArgs : StageArgs
    {
        public Stage SourceStage
        {
            get
            {
                return _stage;
            }
        }
        public StageFinishedArgs(Stage stage)
            : base(stage)
        {

        }
    }

    public abstract class BaseHost : Window, IHost
    {
        public event EventHandler onStageChanged;
        protected bool preventUI = false;
        protected Stage farthestStage = Stage.Selection;
        
        private ListBox lstSteps = null;
        protected List<BaseUserControl> stageUserControls = new List<BaseUserControl>();
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BaseHost()
        {
            AddSteps();
        }

        private void AddSteps()
        {
            //stageUserControls.Add(new ProtocolSelection(Stage.Selection, this));
            ////stageUserControls.Add(new AssayDefinition(Stage.AssayDef, this));
            //stageUserControls.Add(new BarcodeDefinition(Stage.BarcodeDef, this));
            //stageUserControls.Add(new StepMonitor(Stage.StepMonitor, this));
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            foreach (var control in stageUserControls)
            {
                IStageControl iStageControl = (IStageControl)control;
                if (iStageControl != null)
                    iStageControl.onFinished += iStageControl_onFinished;
            }
        }

        void iStageControl_onFinished(object sender, EventArgs e)
        {
            Stage finishedStage = ((StageFinishedArgs)e).SourceStage;
            farthestStage = (Stage)(finishedStage + 1);
            GlobalVars.Instance.FarthestStage = farthestStage;
            NavigateTo(farthestStage);
        }

        protected void NavigateTo(Stage stage)
        {
            log.InfoFormat("Navigate to {0}", stage);
            if (onStageChanged != null)
                onStageChanged(this, new Navigate2Args(stage));
            ListBox lstBox = GetListBox();
            if (lstBox == null)
                return;

            lstBox.SelectedIndex = (int)stage;

        }

        private ListBox GetListBox()
        {
            List<ListBox> lstBoxes = GetChildObjects<ListBox>(this, typeof(ListBox));
            return lstBoxes.First();
        }

        private List<T> GetChildObjects<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, typename));
            }
            return childList;
        }
    }

    public abstract class BaseUserControl : UserControl, IStageControl
    {
        private Stage _stage;
        bool resultIsOk = false;
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BaseUserControl(Stage stage, BaseHost host)
        {
            _stage = stage;
            host.onStageChanged += onStageChanged;
        }
        
        public BaseUserControl()
        {
        }

        public bool ResultIsOk 
        { 
            get
            {
                return resultIsOk;
            }
            set 
            {
                resultIsOk = value;
            }
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void onStageChanged(object sender, EventArgs e)
        {
            Navigate2Args navigate2Args = (Navigate2Args)e;
            
            this.Visibility = navigate2Args.DestStage == _stage ? Visibility.Visible : Visibility.Hidden;
            if(this.Visibility == System.Windows.Visibility.Visible)
                Initialize();
            this.IsEnabled = _stage == GlobalVars.Instance.FarthestStage;
        }

        public Stage CurStage { get { return _stage; } }


        public event EventHandler onFinished;

        public void NotifyFinished()
        {
            if(onFinished != null)
                onFinished(this,new StageFinishedArgs(_stage));
        }
    }
}
