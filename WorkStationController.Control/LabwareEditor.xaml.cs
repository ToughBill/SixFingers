using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LabwareUserControl.xaml
    /// </summary>
    public partial class LabwareEditor : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public LabwareEditor()
        {
            InitializeComponent();
            Labware labware = this.DataContext as Labware;
            //AbsolutionRelativeXPositionConverter.offset = labware.GridID * Worktable.DistanceBetweenAdjacentPins;
            //if( labware.ParentCarrier != null)
            //{
            //    labware.ParentCarrier.Sites[labware.SiteID-1].
            //}
        }


        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be labware
            Labware labware = this.DataContext as Labware;
            if (labware == null)
                throw new InvalidOperationException("DataContext of LabwareEditor must be an instance of Labware");
            PipettorElementManager.Instance.SavePipettorElement(labware);
        }
    }


    public class AbsolutionRelativeXPositionConverter : IValueConverter
    {
        public static int offset = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int relativeValue = int.Parse(value.ToString());
            return relativeValue + offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int absoluteValue = int.Parse(value.ToString());
            return absoluteValue - offset;
        }
    }

    public class AbsolutionRelativYPositionConverter : IValueConverter
    {
        public static int offset = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int relativeValue = int.Parse(value.ToString());
            return relativeValue + offset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int absoluteValue = int.Parse(value.ToString());
            return absoluteValue - offset;
        }
    }

}
