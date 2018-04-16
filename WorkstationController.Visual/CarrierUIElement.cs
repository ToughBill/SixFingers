using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.Uitility;

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
        /// site's default invalid index value
        /// </summary>
        public const int InvalidSiteIndex = -1;
        private int _highLightSiteIndex = InvalidSiteIndex;
        
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
                return _carrier.GridID;
            }

            set
            {
                _carrier.GridID = value;
                base.InvalidateVisual();
            }
        }

        /// <summary>
        /// carrier
        /// </summary>
        public Carrier Carrier
        {
            get
            {
                return _carrier;
            }
            set
            {
                _carrier = value;
            }
        }

        /// <summary>
        /// as property name
        /// </summary>
        public Dimension Dimension
        {
            get
            {
                return _carrier.Dimension;
            }
        }
        /// <summary>
        /// Draw the carrier
        /// </summary>
        /// <param name="drawingVisual"></param>
        override protected void Render(DrawingVisual drawingVisual)
        {
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            //if (Grid == Carrier.undefinedGrid)
            //    return;

            //1 border
            int xPos = GetBoundingRectXStart(_carrier.GridID);//
            int yPos = GetBoundingRectYStart();
            Size sz = new Size(_carrier.Dimension.XLength, _carrier.Dimension.YLength);

            Brush backGroundBrush = new SolidColorBrush(Color.FromArgb(128, _carrier.BackgroundColor.B, _carrier.BackgroundColor.G, _carrier.BackgroundColor.R));
            
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border, backGroundBrush);
            DrawGrid(this.Grid,drawingContext);
            
   
            //2 each site
            for (int i = 0; i < _carrier.Sites.Count; i++ )
            {
                var fillBrsuh = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
                var site = _carrier.Sites[i];
                int xSite = (int)(site.XOffset + xPos);
                int ySite = (int)(site.YOffset + yPos);
                border = _isSelected ? Colors.Blue : Colors.Brown;
                bool bNeedHighLight = i == _highLightSiteIndex;

                Size tmpSZ = new Size(site.XSize, site.YSize);
                Rect rc = new Rect(new Point(xSite, ySite), tmpSZ);
                if (bNeedHighLight)
                {
                    BlowUp(ref rc);
                    border = Colors.DarkGreen;
                    fillBrsuh = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0));
                }
                VisualCommon.DrawRect((int)rc.X, (int)rc.Y, rc.Size, drawingContext, border, fillBrsuh);
            }
            drawingContext.Close();
        }

        private void DrawGrid(int grid, DrawingContext drawingContext)
        {
            int y = (int)Configurations.Instance.Worktable.Size.Height;
            int x = GetGridXPosition(grid);
            VisualCommon.DrawText(new Point(x, y), grid.ToString(), drawingContext);
           
        }

        private void BlowUp(ref Rect rc)
        {
            int blowUnit = (int)(1.5* VisualCommon.containerSize.Height/400.0);
            rc.X -= blowUnit;
            rc.Y -= blowUnit;
            rc.Width += blowUnit*2;
            rc.Height += blowUnit*2;
        }

        private int GetUnderneathPinXStart(int grid)
        {
            return (int)(_worktable.TopLeftPinPosition.X + (grid - 1) * Worktable.DistanceBetweenAdjacentPins);
        }

        /// <summary>
        /// get the bounding rectangle's x start
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int GetBoundingRectXStart(int grid)
        {
            return (int)(GetGridXPosition(grid) - _carrier.XOffset);
        }

        public int GetGridXPosition(int grid)
        {
            return (int)(_worktable.TopLeftPinPosition.X + (grid - 1) * Worktable.DistanceBetweenAdjacentPins);
        }

        /// <summary>
        /// get the bounding rectangle's xStart
        /// </summary>
        /// <returns></returns>
        public int GetBoundingRectXStart()
        {
            return GetBoundingRectXStart(this.Grid);
        }

        private int GetBoundingRectYStart()
        {
            return (int)(_worktable.TopLeftPinPosition.Y - _carrier.YOffset);
        }

        internal void HighLightSiteInShadow(Point ptInCanvas,string labwareTypeName,bool bMouseMove = true)
        {
            SetHighLightFlag(ptInCanvas,labwareTypeName,bMouseMove);
            InvalidateVisual();
        }
      
        private void SetHighLightFlag(Point ptInCanvas,string labwareTypeName, bool bMouseMove = true)
        {
            if (!bMouseMove) 
                return;
            _highLightSiteIndex = GetSiteIndexAcceptsTheLabware(ptInCanvas, labwareTypeName);
        }
        private int GetSiteIndexAcceptsTheLabware(Point ptInCanvas, string labwareTypeName)
        {
            if (Grid <= Carrier.undefinedGrid || !_carrier.AllowedLabwareTypeNames.Contains(labwareTypeName))
                return InvalidSiteIndex;

            int xPos = GetBoundingRectXStart(Grid);
            int yPos = GetBoundingRectYStart();
            Rect rc = VisualCommon.Physic2Visual(xPos, yPos, new Size(_carrier.Dimension.XLength, _carrier.Dimension.YLength));
            if (!rc.Contains(ptInCanvas))
                return InvalidSiteIndex;

            int siteIndex = InvalidSiteIndex;
            for (int i = 0; i < _carrier.Sites.Count;i++ )
            {
                var site = _carrier.Sites[i];
                int xSite = (int)(site.XOffset + xPos);
                int ySite = (int)(site.YOffset + yPos);
                rc = VisualCommon.Physic2Visual(xSite, ySite, new Size(site.XSize, site.YSize));
                if (rc.Contains(ptInCanvas))
                {
                    siteIndex =  i;
                    break;
                }
            }
            return siteIndex;
        }

   
        /// <summary>
        /// find a site which can accept the labware
        /// </summary>
        /// <param name="ptInCanvas"></param>
        /// <param name="labwareTypeName"></param>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public bool GetSiteIDAcceptsTheLabware(Point ptInCanvas, string labwareTypeName, ref int siteID)
        {
            int siteIndex = GetSiteIndexAcceptsTheLabware(ptInCanvas, labwareTypeName);
            if( siteIndex != InvalidSiteIndex)
            {
                siteID = _carrier.Sites[siteIndex].ID;
            }
            return siteIndex != InvalidSiteIndex;
            
        }
    }
}
