using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Diagnostics;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement.ContextMenu;

namespace WorkstationController.VisualElement.Uitility
{
    /// <summary>
    /// response to user's actions and control the render of UI
    /// </summary>
    public class UIMovementsController
    {
        //private bool isDragging = false;
        private System.Windows.Controls.Grid _myCanvas;
        private Point _ptClick;
        private BasewareUIElement _selectedUIElement;
        private BasewareUIElement _uiElementCandidate;
        private bool enableMouseMove = true;
        private bool otherFormNeedPickup = false;
        private DateTime lastClickTime = DateTime.MinValue;
        Vector relativeClickPosition2LeftTop = new Vector(-1, -1);
        readonly Vector notdefinedRelativePosition = new Vector(-1, -1);

        #region events
        /// <summary>
        /// when user double click on labware
        /// </summary>
        public event EventHandler onLabelPreviewChanged;

        /// <summary>
        /// when right button down on labware
        /// </summary>
        public event EventHandler onWareContextMenuFired;
        #endregion
        /// <summary>
        /// new UI element introduced from somewhere, nomarlly from listbox
        /// </summary>
        public BasewareUIElement UIElementCandidate
        {
            get
            {
                return _uiElementCandidate;
            }
            set
            {
                relativeClickPosition2LeftTop = notdefinedRelativePosition;
                _uiElementCandidate = value;
            }
        }

        
       /// <summary>
       /// commands need to pick up a ware.
       /// </summary>
        public bool AllowPickup
        { 
            get
            {
                return otherFormNeedPickup;
            }
            set
            {
                otherFormNeedPickup = value;
                //ClearLastSelection();
            }
        }

        /// <summary>
        /// selected UI element
        /// </summary>
        public BasewareUIElement SelectedElement
        {
            get { return _selectedUIElement; }
            set
            {
                _selectedUIElement = value;
            }
        }

        /// <summary>
        /// ctor, control the grid
        /// </summary>
        /// <param name="grid"></param>
        public UIMovementsController(System.Windows.Controls.Grid grid)
        {
            // TODO: Complete member initialization
            this._myCanvas = grid;
            _myCanvas.PreviewMouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            _myCanvas.PreviewMouseLeftButtonUp += myCanvas_PreviewMouseLeftButtonUp;
            _myCanvas.PreviewMouseRightButtonUp += _myCanvas_PreviewMouseRightButtonUp;
            _myCanvas.IsVisibleChanged += _myCanvas_IsVisibleChanged;
            _myCanvas.MouseMove += myCanvas_MouseMove;
        }

        void _myCanvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
             if (onWareContextMenuFired != null)
                onWareContextMenuFired(this, new ContextEvtArgs(null, new Point(0,0), false));
        }

        void _myCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point ptClick = e.GetPosition(_myCanvas);
            _selectedUIElement = FindSelectedUIElement(ptClick);
            bool bNeed2Show = _selectedUIElement != null;
            if (onWareContextMenuFired != null && bNeed2Show)
            {
                Point ptInScreen = _myCanvas.PointToScreen(ptClick);
                onWareContextMenuFired(this, new ContextEvtArgs(_selectedUIElement.Ware, ptInScreen, bNeed2Show));
            }
            
        }

        /// <summary>
        /// let mycanvas owns the mouse event
        /// </summary>
        public void CaptureMouse()
        {
            _myCanvas.CaptureMouse();
        }

        void myCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //fire context menu event
            if (onWareContextMenuFired != null && _selectedUIElement!= null)
            {
                onWareContextMenuFired(this, new ContextEvtArgs(_selectedUIElement.Ware, new Point(0,0), false));
            }

            //judge double click
            DateTime now = DateTime.Now;
            bool isDoubleClick = e.ClickCount == 2;
            lastClickTime = now;

            //select ui element
            Point ptClick = e.GetPosition(_myCanvas);
            ClearLastSelection();
            _selectedUIElement = FindSelectedUIElement(ptClick);
            if (_selectedUIElement == null)
                return;
           
           
            //process double click event for labwareUIElement
            if(_selectedUIElement is LabwareUIElement && isDoubleClick)
            {
                LabwareUIElement labwareUIElement = _selectedUIElement as LabwareUIElement;
                labwareUIElement.Selected = false;
                onLabelPreviewChanged(this, new LabelChangeEventArgs(labwareUIElement));
                _selectedUIElement = null;
                return;
            }

            if(otherFormNeedPickup)
            {
                _selectedUIElement.HighLighted = true;
                return;
            }
            _ptClick = ptClick;
            SetUIElementSelectedState();
            _myCanvas.CaptureMouse();
        }

        private void SetUIElementSelectedState()
        {
            _selectedUIElement.Selected = true;
            RememberRelativePosition(_selectedUIElement);
            if(_selectedUIElement is LabwareUIElement)
            {
                //relativeClickPosition2LeftTop
                ((LabwareUIElement)_selectedUIElement).Labware.ParentCarrier = null;
            }
        }

        private void RememberRelativePosition(BasewareUIElement baseUIElement)
        {
            if (baseUIElement is LabwareUIElement)
            {
                if (((LabwareUIElement)baseUIElement).Labware.ParentCarrier == null)
                {
                    relativeClickPosition2LeftTop = new Vector(-1, -1);
                    return;
                }
            }
            Point ptIElementInCanvase = baseUIElement.GetLeftTopPositionInCanvas();
            relativeClickPosition2LeftTop = _ptClick - ptIElementInCanvase;
        }

        private void ClearLastSelection()
        {
            foreach (var uiElement in _myCanvas.Children)
            {
                ((BasewareUIElement)uiElement).Selected = false;
                ((BasewareUIElement)uiElement).HighLighted = false;
            }
        }

        void myCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _myCanvas.ReleaseMouseCapture();
            Mouse.OverrideCursor = Cursors.Arrow;

            //clear select flag
            if (_selectedUIElement == null)
                return;

            //pipetting commands need to highlight the labware
            if(otherFormNeedPickup)
                return;


            //here is very tricky, for Carrier, we hope to install them by topleft, 
            //but for labware, we hope to install them by their center.
            Vector vecAdjust = new Vector();
            if(_selectedUIElement is CarrierUIElement)
                vecAdjust = GetAdjustVector();// = e.GetPosition(_myCanvas) - relativeClickPosition2LeftTop;
            WareInstaller.MountThis(_selectedUIElement, e.GetPosition(_myCanvas) - vecAdjust, _myCanvas);
            DeHighlightAllSite();
            _selectedUIElement.Selected = false;
            _selectedUIElement = null;
        }

        void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (otherFormNeedPickup) //other form pick, don't allow mousemove
                return;
           
            Point ptMouse = e.GetPosition(_myCanvas);
            if (e.LeftButton == MouseButtonState.Released)
                return;
            if (!enableMouseMove)
                return;

            bool hasUIElement2Operate = _uiElementCandidate != null || _selectedUIElement != null;
            if (!hasUIElement2Operate)
                return;

            
            if(_selectedUIElement is LabwareUIElement)
            {
                HighlightSiteInShadow(ptMouse, _selectedUIElement.Ware.TypeName);
            }
            ElectCandidate(); //we put the election here to avoid drawing wares out of worktable
            UpdateSelectedElement(ptMouse);
        }


        #region highlight site
        private void HighlightSiteInShadow(Point ptInCanvase,string labwareTypeName, bool bMouseMove = true)
        {
            foreach(var element in _myCanvas.Children)
            {
                if(element is CarrierUIElement)
                {
                    ((CarrierUIElement)element).HighLightSiteInShadow(ptInCanvase, labwareTypeName,bMouseMove);
                }
            }
        }

        private void DeHighlightAllSite()
        {
            HighlightSiteInShadow(new Point(0, 0),"", false);
        }
        #endregion
  
        private void UpdateSelectedElement(Point ptCurrent)
        {
            //adjust position to its center position
            Vector vectorAdjust = GetAdjustVector();
            ptCurrent -= vectorAdjust;
            _selectedUIElement.SetDragPosition(ptCurrent);
            UpdateLabwareUIElements(_selectedUIElement,ptCurrent);
        }

        private Vector GetAdjustVector()
        {
            Size visualSize = _selectedUIElement.VisualSize;
            Vector vector = relativeClickPosition2LeftTop.X < 0 ? new Vector(visualSize.Width / 2, visualSize.Height / 2)
                : relativeClickPosition2LeftTop;
            return vector;
        }

        private void UpdateLabwareUIElements(BasewareUIElement _selectedUIElement,Point ptCurrent)
        {
            if (!(_selectedUIElement is CarrierUIElement))
                return;
            CarrierUIElement carrierUIElement = _selectedUIElement as CarrierUIElement;
            Carrier carrier = carrierUIElement.Carrier;

            int newGrid = VisualCommon.FindCorrespondingGrid(ptCurrent.X);
            int orgGrid = carrier.GridID;
            carrier.GridID = newGrid;
            if (carrier.Labwares.Count == 0)
                return;
            if (newGrid == orgGrid)
                return;

            foreach(Labware labware in carrier.Labwares)
            {
                labware.Refresh();
                Debug.WriteLine("{0} labware hash:{1}", DateTime.Now.ToShortTimeString(), labware.GetHashCode());
            }
        }

        private void ElectCandidate()
        {
            if (_uiElementCandidate == null)
                return;
            _selectedUIElement = _uiElementCandidate;
            _myCanvas.Children.Add(_uiElementCandidate);
            _myCanvas.InvalidateVisual();
            _uiElementCandidate = null;
        }

        //only find LabwareUIElement & CarrierUIElement
        private BasewareUIElement FindSelectedUIElement(Point pt)
        {
            HitTestResult result = VisualTreeHelper.HitTest(_myCanvas, pt);
            if (result == null)
                return null;

            if (otherFormNeedPickup)
                return VisualCommon.FindParent<LabwareUIElement>(result.VisualHit);
            else
            {
                //we give labwareUI element priority
                return VisualCommon.FindParent<BasewareUIElement>(result.VisualHit);
            }
        }

    }
}
