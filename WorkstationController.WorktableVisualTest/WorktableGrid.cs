﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WorkstationController.VisualElement;


namespace WorkstationController.WorktableVisualTest
{
    class WorktableGrid : Grid
    {
        WorktableVisual worktableVisual = null;
        List<UIElement> uiElements = new List<UIElement>();
        public void AttachWorktableVisual(WorktableVisual worktableVisual)
        {
            this.worktableVisual = worktableVisual;
            this.SizeChanged += MyCanvas_SizeChanged;
        }

        void MyCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (worktableVisual == null)
                return;


            UpdateContainerSize(e.NewSize);
            foreach (UIElement uiElement in uiElements)
            {
                ((IRenderableWares)uiElement).Update();
            }
            
            this.InvalidateVisual();
        }

        private void UpdateContainerSize(System.Windows.Size size)
        {
            VisualCommon.containerSize = size;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            if( worktableVisual != null)
                worktableVisual.Draw(dc);
            base.OnRender(dc);
        }
    }
}