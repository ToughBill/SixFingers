using System.Windows.Media;
using System.Windows;
using WorkstationController.Control.Resources;
using System.Collections.Generic;
using WorkstationController.VisualElement;
using System.Windows.Controls;
using System;
using System.Diagnostics;

namespace WorkstationController.Control
{
    /// <summary>
    /// QueryNewLabel.xaml 的交互逻辑
    /// </summary>
    public partial class QueryNewLabelForm : Window
    {
        private List<LabwareUIElement> labwareUIElements;
        private List<CarrierUIElement> carrierUIElements;


        private LabwareUIElement _theOne;
        /// <summary>
        /// ctor
        /// </summary>
        public QueryNewLabelForm()
        {
            InitializeComponent();
        }

       /// <summary>
       /// ctor
       /// </summary>
       /// <param name="uIElementCollection"></param>
       /// <param name="labwareUIElement"></param>
        public QueryNewLabelForm(UIElementCollection uIElementCollection, LabwareUIElement labwareUIElement):this()
        {
            // TODO: Complete member initialization
            //find the current one, the label set cannot be same to one of the other ones'.
            txtLabwareLabel.Text = labwareUIElement.Label;
            try
            {
                labwareUIElements = new List<LabwareUIElement>();
                carrierUIElements = new List<CarrierUIElement>();
                _theOne = labwareUIElement;
                foreach (UIElement element in uIElementCollection)
                {
                    bool isTheOne = element.Equals(_theOne);
                    if (isTheOne)
                        continue;

                    if (element is LabwareUIElement)
                        labwareUIElements.Add(element as LabwareUIElement);
                    else
                        carrierUIElements.Add(element as CarrierUIElement);
                }
            }
            catch(Exception ex)
            {
                ShowWarning(ex.Message);
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string s = txtLabwareLabel.Text;
            if(s == string.Empty)
            {
                ShowWarning(strings.LabelCannotBeEmpty);
                return;
            }
            else
            {
                if( labwareUIElements.Exists(x=>x.Label == s))
                {
                    ShowWarning(strings.LabelAlreadyExist);
                    return;
                }
#if DEBUG
                Debug.WriteLine("before set!");
                DumpDebugInfo();
#endif
                _theOne.Label = s;
#if DEBUG
                
                DumpDebugInfo();
                Debug.WriteLine("after set!");
#endif      
            }
            this.Close();
        }

        private void DumpDebugInfo()
        {
            Debug.WriteLine("current labwareUI hash: {0}", _theOne.GetHashCode());
            //foreach(var element in carrierUIElements)
            //{
            //    Debug.WriteLine("Label: " +  element.Carrier.Labwares[0].Label);
            //    Debug.WriteLine("HashCode:{0}", element.Carrier.Labwares[0].GetHashCode());
            //}
        }

        private void ShowWarning(string s)
        {
            txtHint.Text = s;
            txtHint.Foreground = Brushes.Red;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
