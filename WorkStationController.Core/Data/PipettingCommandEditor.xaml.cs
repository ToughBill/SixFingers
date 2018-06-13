using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.Control.Resources;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.Control
{
    /// <summary>
    /// PipettingCommand.xaml 的交互逻辑
    /// </summary>
    public partial class  PipettingCommandEidtor : UserControl
    {
        Labware _labware;
        bool bOk = false;
        AdvancedPipettingCommand pipettingCommand = null;
        LabwareUIElementFixedSize labwareUIElement = null;
   

        public PipettingCommandEidtor(Labware selectedLabware, bool isAspirate)
        {
            _labware = selectedLabware;
            InitializeComponent();
            this.Loaded += PipettingCommandEidtor_Loaded;
            SetCaption(isAspirate);
        }

        private void SetCaption(bool isAspirate)
        {
 	        lblCommandName.Content = isAspirate ? strings.aspirate : strings.dispense;
        }

        void PipettingCommandEidtor_Loaded(object sender, RoutedEventArgs e)
        {
            labwareUIElement = new LabwareUIElementFixedSize(_labware, new Size(container.ActualWidth, container.ActualHeight));
            container.Children.Add(labwareUIElement);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            
            string sVolume = txtVolume.Text;
            int volume = 0;
            bool bok = int.TryParse(sVolume, out volume);
            if(!bok)
            {
                SetInfo(strings.VolumeMustBeDigital, Brushes.Red);
                return;
            }
            if(volume <= 0)
            {
                SetInfo(strings.VolumeMustBeBiggerThan0, Brushes.Red);
                return;
            }

            if( labwareUIElement.SelectedWellIDs.Count == 0)
            {
                SetInfo(strings.MustSelectSomeWell,Brushes.Red);
                return;
            }
            pipettingCommand = new AdvancedPipettingCommand(_labware.Label,new List<int>(){1},
                labwareUIElement.SelectedWellIDs,
                GetLiquidClass(), 
                ((string)lblCommandName.Content) == strings.aspirate,volume);
            bOk = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private LiquidClass GetLiquidClass()
        {
            //would be:
 	        return (LiquidClass)liquidClasses.SelectedItem;
        }

        private void SetInfo(string text, Brush color)
        {
            txtInfo.Text = text;
            txtInfo.Foreground = color ;
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }


        #region properties
        public bool IsOk
        {
            get
            {
                return bOk;
            }
        }

        public AdvancedPipettingCommand PipettingCommand
        {
            get
            {
                return pipettingCommand;
            }
        }
        #endregion

    }


    
}
