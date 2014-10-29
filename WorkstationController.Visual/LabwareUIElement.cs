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
            if (!_isSelected && carrier == null)
                return;


            int mapGrid = 0;
            if( carrier != null)
            {
                mapGrid = carrier.Grid;
            }

            if (_isSelected)
            {
                mapGrid = VisualCommon.FindCorrespondingGrid(_ptDragPosition.X);
            }


            int pinPos = (mapGrid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.FirstPinPosition.X;
            int xPos = pinPos;
            int yPos = (int)_worktable.FirstPinPosition.Y;
            if (carrier != null)
            {
                xPos = pinPos - (carrier.XOffset);  //get carrier x start pos
                yPos -= carrier.YOffset;
                int siteIndex = _labware.SiteID - 1;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.Position.X;       //get site x start pos
                yPos += (int)site.Position.Y;
            }
            else
            {
                Point ptPhysical = VisualCommon.Convert2PhysicalXY(_ptDragPosition.X, _ptDragPosition.Y);
                xPos = (int)ptPhysical.X;
                yPos = (int)ptPhysical.Y;
                //drawingContext.DrawRectangle(Brushes.Black, null, new Rect(xPos,yPos,10,10));
            }
            
            Size sz = new Size(_labware.Dimension.XLength, _labware.Dimension.YLength);
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);
            VisualCommon.DrawText( new Point( xPos, yPos+sz.Height), _labware.Label,drawingContext);
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            Vector vector = new Vector(xPos + wellDefaultOffsetToSiteMargin, yPos + wellDefaultOffsetToSiteMargin);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var position = _labware.GetPosition(row, col) + vector;
                    VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext, _labware.BackgroundColor);
                }
            }
            drawingContext.Close();
        }



        internal Point GetLeftTopPositionInCanvas()
        {
            var carrier = _labware.ParentCarrier;
            int grid = carrier.Grid;
            int pinPos = (grid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.FirstPinPosition.X;
            int xPos = pinPos;
            int yPos = (int)_worktable.FirstPinPosition.Y;
            xPos = pinPos - (carrier.XOffset);  //get carrier x start pos
            yPos -= carrier.YOffset;
            int siteIndex = _labware.SiteID - 1;
            var site = carrier.Sites[siteIndex];
            xPos += (int)site.Position.X;       //get site x start pos
            yPos += (int)site.Position.Y;
            Rect rc = VisualCommon.Physic2Visual(xPos, yPos, new Size(0, 0));
            return rc.TopLeft;
        }
    }
}
