using System;
using System.Collections.Generic;
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
using WorkstationController.Core.Utility;
using WTPipetting.Navigation;

namespace WTPipetting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :  BaseHost
    {
        StepViewModel stepViewModel = new StepViewModel();
        public MainWindow()
            : base()
        {
            InitializeComponent();
            log.Info("Main window created.");
            this.Loaded += MainWindow_Loaded;
            lstSteps.DataContext = stepViewModel.StepsModel;
        }


        

        #region event
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PipettorElementManager.Instance.Initialize();
            foreach (var userControl in stageUserControls)
                userControlHost.Children.Add(userControl);
            NavigateTo(Stage.SampleDef);
        }
        private void lstSteps_PreviewMouseLeftButtonUp(object sender,
          MouseButtonEventArgs e)
        {
            if (preventUI)
                return;
            var item = ItemsControl.ContainerFromElement(lstSteps, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                Stage stage2Go = ((StepDesc)item.Content).CorrespondingStage;
                if (stage2Go > farthestStage)
                {
                    e.Handled = true;
                }
                else
                {
                    NavigateTo(stage2Go);

                }
            }
        }



        private void btnDitiSetting_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion

      
    }
}
