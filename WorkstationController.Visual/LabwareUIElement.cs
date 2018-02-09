using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

        /// <summary>
        /// the wells to be aspirated from
        /// </summary>
        public List<int> AspirateWellIDs { get; set; }

        
        /// <summary>
        /// the wells to be dispensed to
        /// </summary>
        public List<int> DispenseWellIDs { get; set; }

        /// <summary>
        /// whether the labware is being dragging
        /// </summary>
        public bool Moving { get; set; }
        const int wellDefaultOffsetToSiteMargin = 120;
        private bool blowUp = false;
        private Timer timer = new Timer(1000);
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="labware"></param>
        public LabwareUIElement(Labware labware):base((WareBase)labware)
        {
            this._labware = labware;
            _children.Add(CreateViusal());
            AspirateWellIDs = new List<int>();
            DispenseWellIDs = new List<int>();
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(AspirateWellIDs.Count > 0 || DispenseWellIDs.Count > 0)
            {
                blowUp = !blowUp;
                this.Dispatcher.Invoke(() =>
                {
                    InvalidateVisual();
                });
            }
                
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
            set
            {
                _labware = value;
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
            double xPos = 0, yPos = 0;
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
            Vector vector = new Vector(xPos, yPos);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int wellID = col * rows + row + 1;
                    Color wellColor = Colors.Black;

                    bool bFill = false;
                    if (AspirateWellIDs != null && AspirateWellIDs.Contains(wellID))
                    {
                        wellColor = Colors.Green;
                        bFill = true;
                    }

                    if (DispenseWellIDs != null && DispenseWellIDs.Contains(wellID))
                    {
                        wellColor = Colors.Red;
                        bFill = true;
                    }
                    bFill &= blowUp;
                    var position = _labware.GetPosition(row, col) + vector;
                    VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext, wellColor, bFill);
                }
            }
            drawingContext.Close();
        }

        private void CalculatePositions(ref double xPos, ref double yPos, int mapGrid, Carrier carrier)
        {
            int pinPos = (mapGrid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.TopLeftPinPosition.X;
            xPos = pinPos;
            yPos = _worktable.TopLeftPinPosition.Y;
            if (carrier != null && (!Moving))
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
