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

        public bool IsOk 
        { 
            get 
            {
                return bOk;
            }
        }

        public PipettingCommandEidtor(Labware selectedLabware)
        {
            _labware = selectedLabware;
            InitializeComponent();
            this.Loaded += PipettingCommandEidtor_Loaded;
          
        }

        void PipettingCommandEidtor_Loaded(object sender, RoutedEventArgs e)
        {

            container.Children.Add(new LabwareUIElementFixedSize(_labware, new Size(container.ActualWidth, container.ActualHeight)));
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            bOk = true;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }


    class PipettingSettings
    {
        List<int> _tipsIDUsed;
        List<int> _selectedWellIDs;
        bool _isAspirate;
        int _volumeUL;
        public PipettingSettings(List<int> tipIDUsed, List<int> selectedWellIDs, bool isAspirate, int volumeUL)
        {
            _tipsIDUsed = tipIDUsed;
            _selectedWellIDs = selectedWellIDs;
            _isAspirate = isAspirate;
            _volumeUL = volumeUL;
        }
        public int VolumeUL
        { 
            get
            {
                return _volumeUL;
            }
        }
        public List<int> SelectedWellIDs
        {
            get
            {
                return _selectedWellIDs;
            }
        }
        public List<int> TipsIDsUsed
        {
            get
            {
                return _tipsIDUsed;
            }
        }
        public bool IsAspirate
        {
            get
            {
                return _isAspirate;
            }
        }

    }
}
