using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

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

        
        protected override void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            string carrierLabel = _labware.CarrierLabel;
            Carrier carrier = null;

            int xPos = 0;
            int yPos = 0;
            if (carrier != null)
            {
                xPos = (int)(_worktable.FirstPinPosition.X + carrier.Grid * Worktable.DistanceBetweenAdjacentPins - carrier.XOffset);
                yPos = (int)(_worktable.FirstPinPosition.Y - carrier.YOffset);
                int siteIndex = _labware.SiteID;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.Position.X;
                yPos += (int)site.Position.Y;
            }
            Size sz = new Size(_labware.Dimension.XLength, _labware.Dimension.YLength);
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);
            
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var position = GetPosition(row, col) + new Vector(xPos, yPos);
                    VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext, _labware.BackGroundColor);
                }
            }
            drawingContext.Close();
        }

        private Point GetPosition(int row, int col)
        {
            int xs = (int)_labware.WellsInfo.FirstWellPosition.X;
            int xe = (int)_labware.WellsInfo.LastWellPosition.X;
            double eachXUnit = (xe - xs) / _labware.WellsInfo.NumberOfWellsX;
            double x = (1 + col) * eachXUnit;

            int ys = (int)_labware.WellsInfo.FirstWellPosition.Y;
            int ye = (int)_labware.WellsInfo.LastWellPosition.Y;
            double eachYUnit = (ye - ys) / _labware.WellsInfo.NumberOfWellsY;
            double y = (1 + row) * eachYUnit;
            return new Point(x, y);
        }


   
    }
}
