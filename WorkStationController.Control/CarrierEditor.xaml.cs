using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WorkstationController.Core.Utility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for CarrierEditor.xaml
    /// </summary>
    public partial class CarrierEditor : UserControl
    {
        private ObservableCollection<LabwareCandidate> labwareCandidates;
        /// <summary>
        /// ctor
        /// </summary>
        public CarrierEditor()
        {
            InitializeComponent();
            this.Loaded += CarrierEditor_Loaded;
        }

        void CarrierEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            labwareCandidates = new ObservableCollection<LabwareCandidate>();
            foreach (var labware in PipettorElementManager.Instance.Labwares)
            {
                labwareCandidates.Add(new LabwareCandidate(labware.TypeName, true));
            }
            if (carrier != null)
            {
                foreach (var labwareCandidate in labwareCandidates)
                {
                    labwareCandidate.IsAllowed = carrier.AllowedLabwareTypeNames.Contains(labwareCandidate.TypeName);
                }
                this._colorPicker.SelectedColor = carrier.BackgroundColor;
            }
            lstAllowedLabwares.DataContext = labwareCandidates;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be Carrier
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");

            carrier.AllowedLabwareTypeNames = new ObservableCollection<string>(labwareCandidates.Where(x => x.IsAllowed).Select(x => x.TypeName));
            PipettorElementManager.Instance.SavePipettorElement(carrier);
        }

        private void btnAddSite_Click(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");
            int existSiteCnt = carrier.Sites.Count;
            Site newSite = new Site();
            if (existSiteCnt > 0)
                newSite = (Site)carrier.Sites[0].Clone();
            newSite.ID = existSiteCnt + 1;
            carrier.Sites.Add(newSite);
        }

        private void btnRemoveSite_Click(object sender, RoutedEventArgs e)
        {
            Carrier carrier = this.DataContext as Carrier;
            if (carrier == null)
                throw new InvalidOperationException("DataContext of Carrier must be an instance of Carrier");
            int carrierCnt = carrier.Sites.Count;
            if (carrierCnt == 0)
                return;
            carrier.Sites.RemoveAt(carrierCnt-1); //remove last
        }
    }

    class LabwareCandidate
    {
        public bool IsAllowed { get; set; }
        public string TypeName { get; set; }
        public LabwareCandidate(string typeName, bool isAllowed)
        {
            IsAllowed = isAllowed;
            TypeName = typeName;
        }
    }
}
