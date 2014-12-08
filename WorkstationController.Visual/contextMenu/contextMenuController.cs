using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// controller of ware's context menu.
        /// </summary>
        /// <param name="movementsController"></param>
        public WareContextMenuController(UIMovementsController movementsController)
        {
            movementsController.onWareContextMenuFired += movementsController_onWareContextFired;
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

            contextMenuForm.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            contextMenuForm.Topmost = true;
            if( windowState == ContextWindowState.closed)
                contextMenuForm.Show();
            else
                windowState = ContextWindowState.show;
            contextMenuForm.Left = contextEvtArgs.Position.X;
            contextMenuForm.Top = contextEvtArgs.Position.Y;
        }


        enum ContextWindowState
        {
            closed = 0,
            show
        }
    }
}
