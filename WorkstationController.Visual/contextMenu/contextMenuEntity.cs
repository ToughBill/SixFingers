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
    public class ContextMenuEntity
    {
        private string text;
        private WareCommand wareCommand;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="sText"></param>
        public ContextMenuEntity(string sText)
        {
            text = sText;
        }

        /// <summary>
        /// command's description text
        /// </summary>
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
