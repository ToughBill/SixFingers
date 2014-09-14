using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Visuals;

namespace TestWorktable
{
    class MyCanvas : Canvas
    {
        WorktableVisual worktableVisual = null;

        public void AttachWorktableVisual(WorktableVisual worktableVisual)
        {
            this.worktableVisual = worktableVisual;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            if( worktableVisual != null)
                worktableVisual.Draw(dc);
            base.OnRender(dc);
        }
    }
}
