using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.contextMenu;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.VisualElement.ContextMenu
{
    /// <summary>
    /// process context menu of labware
    /// </summary>
    public class WareContextMenuController : IDisposable
    {
        ContextMenuForm contextMenuForm = new ContextMenuForm();
        ContextWindowState windowState = ContextWindowState.closed;
        WareBase wareBaseClicked = null;
        UIMovementsController _movementsController;

        #region events
        /// <summary>
        /// event when about to edit labware 
        /// </summary>
        public event EventHandler onEditLabware;

        /// <summary>
        /// event when about to edit carrier
        /// </summary>
        public event EventHandler onEditCarrier;
        #endregion

        /// <summary>
        /// controller of ware's context menu.
        /// </summary>
        /// <param name="movementsController"></param>
        public WareContextMenuController(UIMovementsController movementsController)
        {
            _movementsController = movementsController;
            _movementsController.onWareContextMenuFired += movementsController_onWareContextFired;
        }

        /// <summary>
        /// resource cleaner
        /// </summary>
        public void Dispose()
        {
            _movementsController.onWareContextMenuFired -= movementsController_onWareContextFired;
            contextMenuForm.Close();
        }

        private ObservableCollection<ContextMenuEntity> PrepareAllMenuEntities()
        {
            const string delete = "delete";
            const string edit = "edit";
            //ContextMenuEntity deleteItm = new ContextMenuEntity(delete);
            //deleteItm.Command2Do = new WareCommand(delete, typeof(Window));
            //deleteItm.Command2Do.Executed += onDeleteItem;

            ContextMenuEntity editItm = new ContextMenuEntity(edit);
            editItm.Command2Do = new WareCommand(edit, typeof(Window));
            editItm.Command2Do.Executed += onEditItem;

            ObservableCollection<ContextMenuEntity> menuEntities = new ObservableCollection<ContextMenuEntity>();
            //menuEntities.Add(deleteItm);
            menuEntities.Add(editItm);
            return menuEntities;
        }

        private void onEditItem(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if( wareBaseClicked is Labware)
            {
                if( onEditLabware != null)
                    onEditLabware(this, new LabwareEditArgs(wareBaseClicked as Labware));
            }
            else
            {
                if( onEditCarrier != null)
                    onEditCarrier(this, new CarrierEditArgs(wareBaseClicked as Carrier));
            }
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
            wareBaseClicked = contextEvtArgs.ClickedWareBase;
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
         

        }

        enum ContextWindowState
        {
            closed = 0,
            show
        }

        
    }
}
