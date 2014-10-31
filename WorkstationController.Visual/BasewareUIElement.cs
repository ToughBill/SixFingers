using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.VisualElement
{
    public class BasewareUIElement : UIElement
    {
        /// <summary>
        /// Visual collection
        /// </summary>
        protected VisualCollection _children = null;

        /// <summary>
        /// Worktable data
        /// </summary>
        protected Worktable _worktable = null;
        protected bool _isSelected = false;
        protected bool _isHighLighted = false;
        protected WareBase _ware = null;
        protected Point _ptDragPosition;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="ware"></param>
        public BasewareUIElement(WareBase ware)
        {
            _ware = ware;
            _children = new VisualCollection(this);
            this._worktable = Configurations.Instance.Worktable;
            ware.PropertyChanged += ware_PropertyChanged;
        }

        /// <summary>
        /// get inner ware
        /// </summary>
        public WareBase Ware { 
            get
            {
                return _ware;
            } 
        }

        void ware_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        /// <summary>
        /// get physical size
        /// </summary>
        public Size PhysicalSize
        {
            get
            {
                return new Size(_ware.Dimension.XLength, _ware.Dimension.YLength);
            }
        }
        /// <summary>
        /// visual size in pixel
        /// </summary>
        public Size VisualSize
        {
            get
            {
                return VisualCommon.Physic2Visual(PhysicalSize);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_children.Count > 0)
                Render((DrawingVisual)_children[0]);
        }

        /// <summary>
        /// Create visual element,template method.
        /// </summary>
        /// <returns></returns>
        protected Visual CreateViusal()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Render(drawingVisual);
            return drawingVisual;
        }

        /// <summary>
        /// in some case, we need just highLight the ware instead of Selecting it,
        /// because select meaning potential movement.
        /// </summary>
        public bool HighLighted
        {
            get
            {
                return _isHighLighted;
            }
            set
            {
                _isHighLighted = value;
                InvalidateVisual();
            }
        }


        /// <summary>
        /// whether the UIElement is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                InvalidateVisual();
            }
        }


        /// <summary>
        /// derived class overwrite this to give different outlook.
        /// </summary>
        /// <param name="drawingVisual"></param>
        protected virtual void Render(DrawingVisual drawingVisual)
        {

        }


        #region visual container's MUST methods
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

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
        #endregion

        internal void SetDragPosition(Point ptCurrent)
        {
            _ptDragPosition = ptCurrent;
            InvalidateVisual();
        }

        internal Point GetLeftTopPositionInCanvas()
        {
            Carrier carrier = null;
            if (_ware is Labware)
                carrier = ((Labware)_ware).ParentCarrier;
            else
                carrier = (Carrier)_ware;
            int grid = carrier.Grid;
            int pinPos = (grid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.FirstPinPosition.X;
            int xPos = pinPos;
            int yPos = (int)_worktable.FirstPinPosition.Y;
            xPos = pinPos - (carrier.XOffset);  //get carrier x start pos
            yPos -= carrier.YOffset;

            if (_ware is Labware)
            {
                int siteIndex = ((Labware)_ware).SiteID - 1;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.Position.X;       //get site x start pos
                yPos += (int)site.Position.Y;
            }
            Rect rc = VisualCommon.Physic2Visual(xPos, yPos, new Size(0, 0));
            return rc.TopLeft;
        }
    }
}
