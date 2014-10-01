using System;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// Carrier GUI element
    /// </summary>
    public class CarrierUIElement : UIElement, IRenderableWares
    {
        /// <summary>
        /// Carrier data
        /// </summary>
        private Carrier _carrier = null;

        /// <summary>
        /// Visual collection
        /// </summary>
        private VisualCollection _children = null;

        /// <summary>
        /// Worktable data
        /// </summary>
        private Worktable _worktable = null;

        private bool _selected = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="carrier"></param>
        public CarrierUIElement(Carrier carrier)
        {
            if (carrier == null)
                throw new ArgumentNullException("carrier", "carrier could not be null.");

            this._carrier = carrier;
            this._worktable = Configurations.Instance.Worktable;
            this._children = new VisualCollection(this);
            this._children.Add(CreateViusal());
        }

        /// <summary>
        /// Gets or sets the label of the carrier element
        /// </summary>
        public string Label
        {
            get
            {
                return _carrier.Name;
            }
            set
            {
                _carrier.Name = value;
            }
        }

        /// <summary>
        /// Gets the child visuals
        /// </summary>
        public VisualCollection Visuals
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Gets or sets the grid of the carrier
        /// </summary>
        public int Grid
        {
            get
            {
                return _carrier.Grid;
            }

            set
            {
                _carrier.Grid = value;
                Update();
            }
        }

        /// <summary>
        /// whether the UIElement is selected
        /// </summary>
        public bool Selected 
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
            }
        }

        /// <summary>
        /// Rerender the carrier
        /// </summary>
        public void Update()
        {
            if (_children.Count > 0)
                Render((DrawingVisual)_children[0]);
        }

        /// <summary>
        /// Create visual element
        /// </summary>
        /// <returns></returns>
        private Visual CreateViusal()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Render(drawingVisual);
             return drawingVisual;
        }
        
        /// <summary>
        /// Draw the carrier
        /// </summary>
        /// <param name="drawingVisual"></param>
        private void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            //carrier.YLength
            int xPos = (int)(_worktable.FirstPinPosition.X + _carrier.Grid * Worktable.DistanceBetweenAdjacentPins - _carrier.XOffset);
            int yPos = (int)(_worktable.FirstPinPosition.Y - _carrier.XOffset);
            Size sz = new Size(_carrier.XLength, _carrier.YLength);
            Color border = _selected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);
            drawingContext.Close();
        }
    }
}
