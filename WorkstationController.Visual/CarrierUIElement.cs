using System;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// Carrier GUI element
    /// </summary>
    public class CarrierUIElement : BasewareUIElement
    {
        /// <summary>
        /// Carrier data
        /// </summary>
        private Carrier _carrier = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="carrier"></param>
        public CarrierUIElement(Carrier carrier):base((WareBase)carrier)
        {
            if (carrier == null)
                throw new ArgumentNullException("carrier", "carrier could not be null.");

            this._carrier = carrier;
            this._children.Add(CreateViusal()); //we delay the create util now because we need grid property
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
        /// Draw the carrier
        /// </summary>
        /// <param name="drawingVisual"></param>
        override protected void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            if (!_isSelected && Grid == Carrier.undefinedGrid)
                return;

            //carrier.YLength
            int xPos = (int)(_worktable.FirstPinPosition.X + _carrier.Grid * Worktable.DistanceBetweenAdjacentPins - _carrier.XOffset);
            int yPos = (int)(_worktable.FirstPinPosition.Y - _carrier.XOffset);
            Size sz = new Size(_carrier.XLength, _carrier.YLength);
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);
            drawingContext.Close();
        }
    }
}
