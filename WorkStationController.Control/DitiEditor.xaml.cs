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
using WorkstationController.Control.Resources;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for DitiEditor.xaml
    /// </summary>


    public partial class DitiEditor : Window
    {
        Labware _labware;
        LabwareUIElementFixedSize labwareUIElement = null;
        public DitiEditor(Labware selectedLabware)
        {
            _labware = selectedLabware;
            InitializeComponent();
            this.Loaded += DitiEidtor_Loaded;
        }

   
        void DitiEidtor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = 600;
            this.Height = 550;
            labwareUIElement = new LabwareUIElementFixedSize(_labware, new Size(container.ActualWidth, container.ActualHeight));
            container.Children.Add(labwareUIElement);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (labwareUIElement.SelectedWellIDs.Count == 0)
            {
                SetInfo(strings.MustSelectSomeWell, Brushes.Red);
                return;
            }
            this.Close();
        }

        private void SetInfo(string text, Brush color)
        {
            txtInfo.Text = text;
            txtInfo.Foreground = color;
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
