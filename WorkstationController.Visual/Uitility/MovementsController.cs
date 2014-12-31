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
using WorkstationController.Core.Utility;

namespace WorkstationController.VisualElement.Uitility
{
    /// <summary>
    /// response to user's actions and control the render of UI
    /// </summary>
    public class UIMovementsController
    {
        #region definitions
        private System.Windows.Controls.Grid _myCanvas;
        private Point _ptClick;
        private BasewareUIElement _selectedUIElement;
        private BasewareUIElement _uiElementCandidate;
        private bool enableMouseMove = true;
        private bool otherFormNeedPickup = false;
        private DateTime lastClickTime = DateTime.MinValue;
        Vector relativeClickPosition2LeftTop = new Vector(-1, -1);
        readonly Vector notdefinedRelativePosition = new Vector(-1, -1);
        #endregion

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

        #region interface
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
        /// let mycanvas owns the mouse event
        /// </summary>
        public void CaptureMouse()
        {
            _myCanvas.CaptureMouse();
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
        #endregion

        #region ctor
        /// <summary>
        /// ctor, control the grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="existRecipe"></param>
        public UIMovementsController(System.Windows.Controls.Grid grid,Recipe existRecipe)
        {
            // TODO: Complete member initialization
            this._myCanvas = grid;
            InitializeWares(existRecipe);
            _myCanvas.PreviewMouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            _myCanvas.PreviewMouseLeftButtonUp += myCanvas_PreviewMouseLeftButtonUp;
            _myCanvas.PreviewMouseRightButtonUp += _myCanvas_PreviewMouseRightButtonUp;
            _myCanvas.IsVisibleChanged += _myCanvas_IsVisibleChanged;
            _myCanvas.MouseMove += myCanvas_MouseMove;
            PipettorElementManager.Instance.onWareChanged += Instance_onWareChanged;
        }

        void Instance_onWareChanged(object sender, EventArgs e)
        {
            WareBase wareBase = ((WareEditArgs)e).WareBase;
            UpdateWare(wareBase);
        }
        private void InitializeWares(Recipe existRecipe)
        {
            if (existRecipe == null)
                return;

            foreach(Carrier carrier in existRecipe.Carriers)
            {
                foreach(Labware labware in carrier.Labwares)
                {
                    var labwareUIElement = new LabwareUIElement(labware);
                    Grid.SetZIndex(labwareUIElement, 20); //closer to user
                    _myCanvas.Children.Add(labwareUIElement);
                }
                var carrierUIElement = new CarrierUIElement(carrier);
                Grid.SetZIndex(carrierUIElement, 10);
                _myCanvas.Children.Add(carrierUIElement);
            }
        }
        #endregion


        #region update carriers
        /// <summary>
        /// update the ware, once it has been changed in editor
        /// </summary>
        /// <param name="ware"></param>
        public void UpdateWare(WareBase ware)
        {
            List<BasewareUIElement> thisTypeUIElements = FindUIElement(ware.TypeName);
            if (thisTypeUIElements.Count == 0)
                return;
            if(ware.GetType() == typeof(Carrier))
            {
                Carrier carrier = (Carrier)ware;
                thisTypeUIElements.ForEach(x => UpdateCarrier(carrier, (CarrierUIElement)x));
            }
            else
            {
                Labware labware = (Labware)ware;
                thisTypeUIElements.ForEach(x => ReplaceLabware(labware, (LabwareUIElement)x));
            }
        }

        private void ReplaceLabware(Labware exampleLabware, LabwareUIElement labwareUIElement)
        {
            Labware currentLabware = labwareUIElement.Labware;
            currentLabware.CarryInfo(exampleLabware);
        }

        private void UpdateCarrier(Carrier exampleCarrier, CarrierUIElement carrierUIElement)
        {
            Carrier currentCarrier = carrierUIElement.Carrier;
            currentCarrier.CarryInfo(exampleCarrier);
        }

        private List<BasewareUIElement> FindUIElement(string sTypeName)
        {
            List<BasewareUIElement> baseUIElements = new List<BasewareUIElement>();
            foreach(BasewareUIElement baseUIElemenet in _myCanvas.Children)
            {
                if(baseUIElemenet.Ware.TypeName == sTypeName)
                {
                    baseUIElements.Add(baseUIElemenet);
                }
            }
            return baseUIElements;
        }

        #endregion
        #region context menu
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
        #endregion

        #region select UIElement, and prepare move
        void myCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //fire context menu event
            if (onWareContextMenuFired != null && _selectedUIElement!= null)
            {
                onWareContextMenuFired(this, new ContextEvtArgs(_selectedUIElement.Ware, new Point(0,0), false));
            }

            UpdateLastClick();
         
            //select ui element
            Point ptClick = e.GetPosition(_myCanvas);
            ClearLastSelection();
            _selectedUIElement = FindSelectedUIElement(ptClick);
            if (_selectedUIElement == null)
                return;
           
            //process double click event for labwareUIElement
            bool isDoubleClick = e.ClickCount == 2;
            onChangeLabel(isDoubleClick);

            if (otherFormNeedPickup)
            {
                _selectedUIElement.HighLighted = true;
            }
            else //let user move the UIElement
            {
                GetReady4Move(ptClick);
              
            }
        }
       
        private void GetReady4Move(Point ptClick)
        {
            _ptClick = ptClick;
            _selectedUIElement.Selected = true;
            RememberRelativePosition(_selectedUIElement);
            ForgetLabwareParent();
            _myCanvas.CaptureMouse();
        }

        private void ForgetLabwareParent()
        {
            if (_selectedUIElement is LabwareUIElement) //forget the parent
            {
                //relativeClickPosition2LeftTop
                ((LabwareUIElement)_selectedUIElement).Labware.ParentCarrier = null;
            }
        }

        private void UpdateLastClick()
        {
            lastClickTime = DateTime.Now;
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
        #endregion

        #region move and mount

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
        private void UpdateSelectedElement(Point ptCurrent)
        {
            //adjust position to its center position
            Vector vectorAdjust = GetAdjustVector();
            ptCurrent -= vectorAdjust;
            _selectedUIElement.SetDragPosition(ptCurrent);
            UpdateLabwareUIElements(_selectedUIElement, ptCurrent);
        }

        private void UpdateLabwareUIElements(BasewareUIElement _selectedUIElement, Point ptCurrent)
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

            foreach (Labware labware in carrier.Labwares)
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
                var labwareUIElement = VisualCommon.FindParent<LabwareUIElement>(result.VisualHit);
                if (labwareUIElement != null)
                    return labwareUIElement;
                else
                    return VisualCommon.FindParent<CarrierUIElement>(result.VisualHit);
            }
        }

        private Vector GetAdjustVector()
        {
            Size visualSize = _selectedUIElement.VisualSize;
            Vector vector = relativeClickPosition2LeftTop.X < 0 ? new Vector(visualSize.Width / 2, visualSize.Height / 2)
                : relativeClickPosition2LeftTop;
            return vector;
        }

        #endregion

        #region double click change label
        private void onChangeLabel(bool isDoubleClick)
        {
            if (!isDoubleClick)
                return;
            if (!(_selectedUIElement is LabwareUIElement))
                return;
            LabwareUIElement labwareUIElement = _selectedUIElement as LabwareUIElement;
            labwareUIElement.Selected = false;
            if (onLabelPreviewChanged != null)
                onLabelPreviewChanged(this, new LabelChangeEventArgs(labwareUIElement));
            _selectedUIElement = null;
            return;
        }
        #endregion 

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
    }
}
