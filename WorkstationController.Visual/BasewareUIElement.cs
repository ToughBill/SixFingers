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
    /// <summary>
    /// base uiElement
    /// </summary>
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

        /// <summary>
        /// whether the ui element is selected
        /// </summary>
        protected bool _isSelected = false;

        /// <summary>
        /// whether the ui element is highlighted
        /// </summary>
        protected bool _isHighLighted = false;

        /// <summary>
        /// the inner ware data
        /// </summary>
        protected WareBase _ware = null;

        /// <summary>
        /// drag position
        /// </summary>
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

        
        /// <summary>
        /// draw method
        /// </summary>
        /// <param name="drawingContext"></param>
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

        /// <summary>
        /// Provide a required override for the VisualChildrenCount property.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        /// <summary>
        ///Provide a required override for the GetVisualChild method. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
            int grid = carrier.GridID;
            int pinPos = (grid - 1) * Worktable.DistanceBetweenAdjacentPins + (int)_worktable.TopLeftPinPosition.X;
            double xPos = pinPos;
            double yPos = _worktable.TopLeftPinPosition.Y;
            xPos = pinPos - (carrier.XOffset);  //get carrier x start pos
            yPos -= carrier.YOffset;

            if (_ware is Labware)
            {
                int siteIndex = ((Labware)_ware).SiteID - 1;
                var site = carrier.Sites[siteIndex];
                xPos += (int)site.XOffset;       //get site x start pos
                yPos += (int)site.YOffset;
            }
            Rect rc = VisualCommon.Physic2Visual(xPos, yPos, new Size(0, 0));
            return rc.TopLeft;
        }
    }
}
