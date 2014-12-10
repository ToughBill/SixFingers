using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.VisualElement.contextMenu;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.VisualElement.ContextMenu
{
    /// <summary>
    /// process context menu of labware
    /// </summary>
    public class WareContextMenuController
    {
        ContextMenuForm contextMenuForm = new ContextMenuForm();
        ContextWindowState windowState = ContextWindowState.closed;
        BasewareUIElement basewareUIElement = null;
        /// <summary>
        /// controller of ware's context menu.
        /// </summary>
        /// <param name="movementsController"></param>
        public WareContextMenuController(UIMovementsController movementsController)
        {
            movementsController.onWareContextMenuFired += movementsController_onWareContextFired;
            
        }

        private ObservableCollection<ContextMenuEntity> PrepareAllMenuEntities()
        {
            const string delete = "delete";
            const string edit = "edit";
            ContextMenuEntity deleteItm = new ContextMenuEntity(delete);
            deleteItm.Command2Do = new WareCommand(delete, typeof(Window));
            deleteItm.Command2Do.Executed += onDeleteItem;

            ContextMenuEntity editItm = new ContextMenuEntity(edit);
            editItm.Command2Do = new WareCommand(edit, typeof(Window));
            editItm.Command2Do.Executed += onEditItem;

            ObservableCollection<ContextMenuEntity> menuEntities = new ObservableCollection<ContextMenuEntity>();
            menuEntities.Add(deleteItm);
            menuEntities.Add(editItm);
            return menuEntities;
        }

        private void onEditItem(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            //throw new NotImplementedException();
            MessageBox.Show("On Edit item");
        }

        void onDeleteItem(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void movementsController_onWareContextFired(object sender, EventArgs e)
        {
            ContextEvtArgs contextEvtArgs = e as ContextEvtArgs;
            if (!contextEvtArgs.Need2Show)
            {
                contextMenuForm.Hide();
                windowState = ContextWindowState.closed;
                return;
            }
            basewareUIElement = contextEvtArgs.Element;
            contextMenuForm.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            contextMenuForm.Topmost = true;
            contextMenuForm.Left = contextEvtArgs.Position.X;
            contextMenuForm.Top = contextEvtArgs.Position.Y;
            if (windowState == ContextWindowState.closed)
            {
                contextMenuForm.Show();
                var menuEntities = PrepareAllMenuEntities();
                contextMenuForm.SetMenus(menuEntities);
            }
            else
                windowState = ContextWindowState.show;
         
            Debug.WriteLine("x:{0}", contextEvtArgs.Position.X);
            Debug.WriteLine("y:{0}", contextEvtArgs.Position.Y);
        }

        enum ContextWindowState
        {
            closed = 0,
            show
        }
    }
}
