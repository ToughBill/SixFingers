using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WorkstationController.Core.Utility
{
    public class CommandsManager
    {

        private static RoutedUICommand _save_pipettorElements = null;

        // Script execute commands
        private static RoutedUICommand _start_script = null;
        private static RoutedUICommand _resume_script = null;
        private static RoutedUICommand _stop_script = null;

        // Labware related commands
        private static RoutedUICommand _edit_labware = null;
        private static RoutedUICommand _new_labware = null;
        private static RoutedUICommand _duplicate_labware = null;
        private static RoutedUICommand _delete_labware = null;

        //diti related commands
        private static RoutedUICommand _set_currentDiti = null;
        private static RoutedUICommand _set_DitiPosition = null;

        // carrier related commands
        private static RoutedUICommand _edit_carrier = null;
        private static RoutedUICommand _new_carrier = null;
        private static RoutedUICommand _duplicate_carrier = null;
        private static RoutedUICommand _delete_carrier = null;


        /// <summary>
        /// Save command
        /// </summary>
        public static RoutedUICommand SavePipettorElements
        {
            get 
            {
                return _save_pipettorElements; 
            }
            set
            {
                _save_pipettorElements = value;
            }
        }

        /// <summary>
        /// Start Script command
        /// </summary>
        public static RoutedUICommand StartScript
        {
            get { return _start_script; }
            set
            {
                _start_script = value;
            }
        }

        /// <summary>
        /// Resume Script command
        /// </summary>
        public static RoutedUICommand ResumeScript
        {
            get { return _resume_script; }
            set
            {
                _resume_script = value;
            }
        }

        /// <summary>
        /// Stop Script command
        /// </summary>
        public static RoutedUICommand StopScript
        {
            get { return _stop_script; }
            set
            {
                _stop_script = value;
            }
        }

        #region labware related

        /// <summary>
        /// Edit Labware command
        /// </summary>
        public static RoutedUICommand EditLabware
        {
            get { return _edit_labware; }
            set
            {
                _edit_labware = value;
            }
        }


        public static RoutedUICommand SetAsCurrentDiti
        {
            get { return _set_currentDiti; }
            set
            {
                _set_currentDiti = value;
            }
        }


        public static RoutedUICommand SetDitiPosition
        {
            get { return _set_DitiPosition; }
            set
            {
                _set_DitiPosition = value;
            }
        }

        /// <summary>
        /// New Labware command
        /// </summary>
        public static RoutedUICommand NewLabware
        {
            get { return _new_labware; }
            set
            {
                _new_labware = value;
            }
        }

        /// <summary>
        /// Duplicate Labware command
        /// </summary>
        public static RoutedUICommand DuplicateLabware
        {
            get { return _duplicate_labware; }
            set
            {
                _duplicate_labware = value;
            }
        }

        /// <summary>
        /// Delete Labware command
        /// </summary>
        public static RoutedUICommand DeleteLabware
        {
            get { return _delete_labware; }
            set
            {
                _delete_labware = value;
            }
        }

        #endregion

        #region carrier related

        /// <summary>
        /// Edit Carrier command
        /// </summary>
        public static RoutedUICommand EditCarrier
        {
            get { return _edit_carrier; }
            set
            {
                _edit_carrier = value;
            }
        }

        /// <summary>
        /// New Carrier command
        /// </summary>
        public static RoutedUICommand NewCarrier
        {
            get { return _new_carrier; }
            set
            {
                _new_carrier = value;
            }
        }

        /// <summary>
        /// Duplicate Carrier command
        /// </summary>
        public static RoutedUICommand DuplicateCarrier
        {
            get { return _duplicate_carrier; }
            set
            {
                _duplicate_carrier = value;
            }
        }

        /// <summary>
        /// Delete Carrier command
        /// </summary>
        public static RoutedUICommand DeleteCarrier
        {
            get { return _delete_carrier; }
            set
            {
                _delete_carrier = value;
            }
        }
        #endregion
    }
}
