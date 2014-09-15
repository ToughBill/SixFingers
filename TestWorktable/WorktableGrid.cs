using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Visuals;

namespace TestWorktable
{
    class WorktableGrid : Grid
    {
        WorktableVisual worktableVisual = null;
        List<CarrierUIElement> carrierUIElements = new List<CarrierUIElement>();
        public void AttachWorktableVisual(WorktableVisual worktableVisual)
        {
            this.worktableVisual = worktableVisual;
            this.SizeChanged += MyCanvas_SizeChanged;
        }

        void MyCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (worktableVisual == null)
                return;

            foreach (CarrierUIElement carrierUI in carrierUIElements)
                carrierUI.Update();

            UpdateContainerSize(e.NewSize);
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
