using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkStationController.Core.Data;

namespace Visuals
{
    public class CarrierUIElement:UIElement
    {
        Carrier carrier;
        private VisualCollection _children;
        private Worktable worktable = null;
        public CarrierUIElement(Carrier carrier)
        {
            this.carrier = carrier;
            worktable = Configurations.Instance.Worktable;
            _children = new VisualCollection(this);
            _children.Add(CreateViusal());
        }

        private Visual CreateViusal()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Render(drawingVisual);
             return drawingVisual;
        }
        public void Update()
        {
            if (_children.Count > 0)
                Render((DrawingVisual)_children[0]);
        }

        private void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            //carrier.YLength
            int xPos = (int)(worktable.FirstPinPosition.X + carrier.Grid * Worktable.distanceBetweenAdjacentPins - carrier.Offset.X);
            int yPos = (int)(worktable.FirstPinPosition.Y - carrier.Offset.X);
            Size sz = new Size(carrier.XLength, carrier.YLength);
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, Colors.Black);
            drawingContext.Close();
          
        }
        public string Label
        {
            get
            {
                return carrier.Name;
            }
            set
            {
                carrier.Name = value;
            }
        }
        public VisualCollection Visuals
        {
            get
            {
                return _children;
            }
        }

        public int Grid
        {
            get
            {
                return carrier.Grid;
            }
            set
            {
                carrier.Grid = value;
                Update();
            }
        }

       
      
     
    }
}
