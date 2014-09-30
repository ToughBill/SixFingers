using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorkstationController.VisualElement
{
    public interface IRenderableWares
    {
        void Update();
        Point RenderOffset{ get; set; }
    }
    
}
