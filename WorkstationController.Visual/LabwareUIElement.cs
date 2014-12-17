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

        /// <summary>
        /// no well is selected;
        /// </summary>
        Labware _labware;

        const int wellDefaultOffsetToSiteMargin = 120;

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
        /// inner data
        /// </summary>
		public Labware Labware
        {
            get
            {
                return _labware;
            }
        }        

        /// <summary>
        /// redraw
        /// </summary>
        /// <param name="drawingVisual"></param>
        protected override void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Carrier carrier = _labware.ParentCarrier ;
            
            if (!NeedMoveOrOnACarrier())
                return;

            int mapGrid = 0;
            if( carrier != null)
            {
                mapGrid = carrier.GridID;
            }

            if (_isSelected)
            {
                mapGrid = VisualCommon.FindCorrespondingGrid(_ptDragPosition.X);
            }


            //calculate positioon & draw border rectangle
            int xPos = 0, yPos = 0;
            CalculatePositions(ref xPos, ref yPos, mapGrid, carrier);
            Size sz = new Size(_labware.Dimension.XLength, _labware.Dimension.YLength);
            Color border = NeedHighLight() ? Colors.Blue : Colors.Black;
            Color background = _labware.BackgroundColor;
            background = Color.FromArgb(160,background.R,background.G,background.B);
            _labware.BackgroundColor = background;
            Brush brush = new SolidColorBrush(_labware.BackgroundColor);
            int thickness = NeedHighLight() ? 2 : 1;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border, brush, thickness);
            VisualCommon.DrawText( new Point( xPos, yPos+sz.Height), _labware.Label,drawingContext);

            //draw wells
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            Vector vector = new Vector(xPos + wellDefaultOffsetToSiteMargin, yPos + wellDefaultOffsetToSiteMargin);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var position = _labware.GetPosition(row, col) + vector;
                    VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext,Colors.Black);
                }
            }
            drawingContext.Close();
        }

        private void CalculatePositions(ref int xPos, ref int yPos, int mapGrid, Carrier carrier)
        {
            int pinPos = (mapGrid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.FirstPinPosition.X;
            xPos = pinPos;
            yPos = (int)_worktable.FirstPinPosition.Y;
            if (carrier != null)
            {
                xPos = pinPos - (carrier.XOffset);  //get carrier x start pos
                yPos -= carrier.YOffset;
                int siteIndex = _labware.SiteID - 1;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.XOffset;          //get site x start pos
                yPos += (int)site.YOffset;
            }
            else
            {
                Point ptPhysical = VisualCommon.Convert2PhysicalXY(_ptDragPosition.X, _ptDragPosition.Y);
                xPos = (int)ptPhysical.X;
                yPos = (int)ptPhysical.Y;
            }
        }

        private bool NeedHighLight()
        {
            return _isHighLighted || _isSelected;
        }

        private bool NeedMoveOrOnACarrier()
        {
            Carrier carrier = _labware.ParentCarrier ;
            return _isSelected || carrier != null;
        }
        
    }
}
