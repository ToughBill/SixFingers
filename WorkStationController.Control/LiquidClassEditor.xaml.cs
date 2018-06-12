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
using WorkstationController.Hardware;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for LiquidClassEditor.xaml
    /// </summary>
    public partial class LiquidClassEditor : BaseEditor
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public LiquidClassEditor(NewInformationHandler newInfoHandler)
            :base(newInfoHandler)
        {
            InitializeComponent();
        }

        private void OnBtnSaveClick(object sender, RoutedEventArgs e)
        {
            
            // The DataContext must be LiquidClass

            LiquidClass liquidClass = this.DataContext as LiquidClass;
            if (liquidClass == null)
                throw new InvalidOperationException("DataContext of LiquiClassEditor must be an instance of LiquidClass");

           
            try
            {
                CheckSpeedsValidity(liquidClass);
                PipettorElementManager.Instance.SavePipettorElement(liquidClass);
            }
            catch(Exception ex)
            {
                 if (newInfoHandler != null)
                    newInfoHandler(ex.Message, true);
            }

        }

        private void CheckSpeedsValidity(LiquidClass liquidClass)
        {
            int maxSpeed = TeachingControllerDelegate.Instance.Controller.MaxPipettingSpeed;
            CheckSpeedValidity(liquidClass.AspirationSinglePipetting.AspirationSpeed, maxSpeed, "AspirationSpeed");
            CheckSpeedValidity(liquidClass.DispenseSinglePipetting.DispenseSpeed, maxSpeed, "DispenseSpeed");
        }

        private void CheckSpeedValidity(double speed, double maxSpeed, string description)
        {
            if (speed <= 0 || speed > maxSpeed)
                throw new Exception(string.Format("{0} 必须在0~{1}之间。",description, maxSpeed));
        }
      
    }
}
