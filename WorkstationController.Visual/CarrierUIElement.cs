using System;
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
        private Site _siteNeedHighLight = null;

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
                InvalidateVisual();
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

            int mapGrid = _carrier.Grid;
            if(_isSelected)
            {
                mapGrid = VisualCommon.FindCorrespondingGrid(_ptDragPosition.X);
            }
            
            //1 border
            int xPos = GetBoundingRectXStart(mapGrid);//
            int yPos = GetBoundingRectYStart();
            Size sz = new Size(_carrier.Dimension.XLength, _carrier.Dimension.YLength);
            Color border = _isSelected ? Colors.Blue : Colors.Black;
            VisualCommon.DrawRect(xPos, yPos, sz, drawingContext, border);

            //2 each site
            foreach(Site site in _carrier.Sites)
            {
                int xSite = (int)(site.Position.X + xPos);
                int ySite = (int)(site.Position.Y + yPos);
                border = _isSelected ? Colors.Blue : Colors.LightGray;
                bool bNeedHighLight = site == _siteNeedHighLight;
                
                Size tmpSZ = new Size(site.Size.Width,site.Size.Height);
                Rect rc = new Rect(new Point(xSite, ySite), tmpSZ);
                if (bNeedHighLight)
                {
                    BlowUp(ref rc);
                    border = Colors.DarkGreen;
                }
                VisualCommon.DrawRect((int)rc.X, (int)rc.Y,rc.Size, drawingContext, border);
            }
            drawingContext.Close();
        }

        private void BlowUp(ref Rect rc)
        {
            int blowUnit = (int)(15* VisualCommon.containerSize.Height/400.0);
            rc.X -= blowUnit;
            rc.Y -= blowUnit;
            rc.Width += blowUnit*2;
            rc.Height += blowUnit*2;
        }

        public int GetBoundingRectXStart(int grid)
        {
            return (int)(_worktable.FirstPinPosition.X + (grid - 1) * Worktable.DistanceBetweenAdjacentPins - _carrier.XOffset);
        }

        public int GetBoundingRectXStart()
        {
            return GetBoundingRectXStart(this.Grid);
        }

        private int GetBoundingRectYStart()
        {
            return (int)(_worktable.FirstPinPosition.Y - _carrier.YOffset);
        }

        internal void HighLightSiteInShadow(Point ptInCanvas,string labwareTypeName,bool bMouseMove = true)
        {
            SetHighLightFlag(ptInCanvas,labwareTypeName,bMouseMove);
            InvalidateVisual();
        }

        private void RemoveHighLightFlag()
        {
            _siteNeedHighLight = null;
        }
        private void SetHighLightFlag(Point ptInCanvas,string labwareTypeName, bool bMouseMove = true)
        {
            RemoveHighLightFlag();
            //if not from mousemove, we need to remove all the highLight flag.
            if (!bMouseMove) 
                return;

            if (Grid != Carrier.undefinedGrid)
            {
                int xPos = GetBoundingRectXStart(Grid);//
                int yPos = GetBoundingRectYStart();
                Rect rc = VisualCommon.Physic2Visual(xPos, yPos, new Size(_carrier.Dimension.XLength, _carrier.Dimension.YLength));
                if (!rc.Contains(ptInCanvas))
                    return;
                foreach (Site site in _carrier.Sites)
                {
                    if (!site.AllowedLabwareTypeNames.Contains(labwareTypeName))
                        continue;

                    int xSite = (int)(site.Position.X + xPos);
                    int ySite = (int)(site.Position.Y + yPos);
                    rc = VisualCommon.Physic2Visual(xSite, ySite, site.Size);
                    if (rc.Contains(ptInCanvas))
                    {
                        _siteNeedHighLight = site;
                        break;
                    }
                }
            }
        }

    }
}
