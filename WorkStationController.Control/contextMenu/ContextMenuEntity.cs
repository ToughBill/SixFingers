using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkstationController.Control
{
    /// <summary>
    /// item of context menu
    /// </summary>
    public class ContextMenuEntity
    {
        private string _header;
        private WareCommand _wareCommand;
        private bool _isEnabled = true;

        public string Header 
        { 
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public WareCommand WareCommand
        {
            get
            {
                return _wareCommand;
            }
            set
            {
                _wareCommand = value;
            }
        }
    }
}
