using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// interface for redrawing
    /// </summary>
    public interface IRenderableWares
    {
        /// <summary>
        /// when called, items redraw themselfs
        /// </summary>
        void Update();
    }
    
}
