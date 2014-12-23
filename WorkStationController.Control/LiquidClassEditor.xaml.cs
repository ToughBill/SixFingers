using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for LiquidClassEditor.xaml
    /// </summary>
    public partial class LiquidClassEditor : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LiquidClassEditor()
        {
            InitializeComponent();
        }

        private void OnBtnSaveClick(object sender, RoutedEventArgs e)
        {
            // The DataContext must be LiquidClass
            LiquidClass liquidClass = this.DataContext as LiquidClass;
            if (liquidClass == null)
                throw new InvalidOperationException("DataContext of LiquiClassEditor must be an instance of LiquidClass");
            InstrumentsManager.Instance.SaveInstrument(liquidClass);
        }
    }
}
