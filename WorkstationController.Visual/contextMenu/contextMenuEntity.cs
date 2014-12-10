using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WorkstationController.VisualElement.contextMenu
{
    /// <summary>
    /// context menu item entity
    /// </summary>
    public class ContextMenuEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// notifier property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private string text;

        private WareCommand wareCommand;

        public ContextMenuEntity(string sText)
        {
            text = sText;
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// command to be executed when item clicked
        /// </summary>
        public WareCommand Command2Do
        {
            get { return wareCommand; }
            set { wareCommand = value; }
        }
    }
}
