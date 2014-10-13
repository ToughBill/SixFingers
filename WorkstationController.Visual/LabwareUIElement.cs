using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// labware on carrier
    /// </summary>
    public class LabwareUIElement : BasewareUIElement
    {
        Labware _labware;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="labware"></param>
        public LabwareUIElement(Labware labware):base((WareBase)labware)
        {
            this._labware = labware;
            _children.Add(CreateViusal());
        }

        /// <summary>
        /// Gets or sets the label of the ware
        /// </summary>
        public string Label
        {
            get
            {
                return _labware.Label;
            }
            set
            {
                _labware.Label = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// redraw
        /// </summary>
        /// <param name="drawingVisual"></param>
        protected override void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Carrier carrier = null;
            if (!_isSelected && _labware.CarrierGrid == Carrier.undefinedGrid)
                return;

            int mapGrid = _labware.CarrierGrid;
            if (_isSelected)
            {
                mapGrid = VisualCommon.FindCorrespondingGrid(_ptDragPosition.X);
            }


            int xPos = (mapGrid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.FirstPinPosition.X;
            int yPos = (int)_worktable.FirstPinPosition.Y;
            if (carrier != null)
            {
                xPos = xPos + carrier.XOffset;
                yPos += carrier.YOffset;
                int siteIndex = _labware.SiteID;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.Position.X;
                yPos += (int)site.Position.Y;
            }
            else
            {
                xPos -= _labware.Dimension.XLength / 2;
                yPos -= 137;
            }
            
            Size sz = new Size(_labware.Dimension.XLength, _labware.Dimension.YLength);
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);
            VisualCommon.DrawText( new Point( xPos, yPos+sz.Height), _labware.Label,drawingContext);
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            Vector vector = new Vector(xPos, yPos);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var position = GetPosition(row, col) + vector;
                    VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext, _labware.BackgroundColor);
                }
            }
            drawingContext.Close();
        }

        private Point GetPosition(int row, int col)
        {
            int xs = 0;
            int xe = (int)_labware.Dimension.XLength;
            double eachXUnit = (xe - xs) / (1 + _labware.WellsInfo.NumberOfWellsX);
            double x = (1 + col) * eachXUnit;

            int ys = (int)_labware.WellsInfo.FirstWellPosition.Y;
            int ye = (int)_labware.WellsInfo.LastWellPosition.Y;
            double eachYUnit = (ye - ys) / (1+_labware.WellsInfo.NumberOfWellsY);
            double y = (1 + row) * eachYUnit;
            return new Point(x, y);
        }


   
    }
}
