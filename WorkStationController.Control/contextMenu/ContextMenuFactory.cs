using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WorkstationController.Control.Resources;

namespace WorkstationController.Control.contextCommands
{
    public class ContextMenuFactory
    {
        private List<ContextMenuEntity> _contextMenus = null;
        private static ContextMenuFactory _instance = null;
        public static ContextMenuFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ContextMenuFactory();
                }
                return _instance;
            }
        }

        private ContextMenuFactory()
        {
            _contextMenus = new List<ContextMenuEntity>();
            WareCommand commandDelete = new WareCommand();
            ContextMenuEntity delete = new ContextMenuEntity() { Header = strings.Delete, WareCommand = commandDelete };
            
            WareCommand commandEdit = new WareCommand();
            ContextMenuEntity edit = new ContextMenuEntity() { Header = strings.Edit, WareCommand = commandEdit };
            _contextMenus.Add(delete);
            _contextMenus.Add(edit);
        }

        public List<ContextMenuEntity> MenuEntitys
        {
            get
            {
                return _contextMenus;
            }
        }
    }
}
