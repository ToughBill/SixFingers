using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WTPipetting.Data;
using WTPipetting.Navigation;
using WTPipetting.Utility;

namespace WTPipetting.StageControls
{
    /// <summary>
    /// Interaction logic for StepMonitorForm.xaml
    /// </summary>
    public partial class StepMonitorForm : BaseUserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public StepMonitorForm(Stage stage, BaseHost host)
            : base(stage, host)
        {
            InitializeComponent();
        }


        protected override void Initialize()
        {
            base.Initialize();
            InitStepsInfo();
        }

        private void InitStepsInfo()
        {
            var stepsDef = ProtocolManager.Instance.SelectedProtocol.StepsDefinition;
            ObservableCollection<StepDefinitionWithProgressInfo> stepsDefWithProgressInfo = new ObservableCollection<StepDefinitionWithProgressInfo>();
            //timeEstimation = new TimeEstimation();
            
            stepsDefWithProgressInfo.Clear();
            foreach (var stepDef in stepsDef)
            {
                var stepDefEx = new StepDefinitionWithProgressInfo(stepDef);
                
                stepsDefWithProgressInfo.Add(stepDefEx);
            }
            //timeInfo.DataContext = timeEstimation;
            lvProtocol.ItemsSource = stepsDefWithProgressInfo;
        }
    }
}
