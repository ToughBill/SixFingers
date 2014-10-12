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
                _uiElementCandidate = value;
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
            _myCanvas.MouseMove += myCanvas_MouseMove;
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
            //PreventMouseMove();
            Point ptClick = e.GetPosition(_myCanvas);
            _selectedUIElement = FindSelectedUIElement(ptClick);
            if (_selectedUIElement != null)
            {
                _selectedUIElement.Selected = true;
                _ptClick = ptClick;
                Mouse.OverrideCursor = Cursors.Hand;
                _myCanvas.CaptureMouse();
            }
           // AllowMouseMove();
        }

        void myCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _myCanvas.ReleaseMouseCapture();
            Mouse.OverrideCursor = Cursors.Arrow;
            //clear select flag
            Debug.WriteLine("Clear selection");
            if (_selectedUIElement == null)
                return;
            WareInstaller.MountThis(_selectedUIElement,e.GetPosition(_myCanvas),_myCanvas);
            DeHighlightAllSite();
            _selectedUIElement = null;
        }


        void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptMouse = e.GetPosition(_myCanvas);
            if (e.LeftButton == MouseButtonState.Released)
                return;
            if (!enableMouseMove)
                return;
            
            
            if (ptMouse.X < 0 || ptMouse.X > _myCanvas.ActualWidth)
                return;

            if (ptMouse.Y < 0 || ptMouse.Y > _myCanvas.ActualHeight)
                return;
            
            bool hasUIElement2Operate = _uiElementCandidate != null || _selectedUIElement != null;
            if (!hasUIElement2Operate)
                return;
            
            if(_selectedUIElement is LabwareUIElement)
            {
                HighlightSiteInShadow(ptMouse, _selectedUIElement.Ware.TypeName);
            }
            
            //double actualWidth = workingElement.RenderSize.Width;
            //double actualHeight = workingElement.RenderSize.Height;
            //if (ptClick.X < 0)
            //    ptClick.X = 0;
            //if (ptClick.X + actualWidth > myCanvas.ActualWidth)
            //    ptClick.X = myCanvas.ActualWidth - actualWidth;
            //if (ptClick.Y < 0)
            //    ptClick.Y = 0;
            //if (ptClick.Y + actualHeight > myCanvas.ActualHeight)
            //    ptClick.Y = myCanvas.ActualHeight - actualHeight;
            ElectCandidate();
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
            _selectedUIElement.SetDragPosition(ptCurrent);
            _selectedUIElement.InvalidateVisual();
        }

        private void ElectCandidate()
        {
            if (_uiElementCandidate == null)
                return;
            Debug.WriteLine("ElectCandidate");
            SelectedElement = UIElementCandidate;
            _myCanvas.Children.Add(UIElementCandidate);
            _myCanvas.InvalidateVisual();
            UIElementCandidate = null;
        }

        //only find LabwareUIElement & CarrierUIElement
        private BasewareUIElement FindSelectedUIElement(Point pt)
        {
            HitTestResult result = VisualTreeHelper.HitTest(_myCanvas, pt);
            if (result == null)
                return null;
            return VisualCommon.FindParent<BasewareUIElement>(result.VisualHit); 
        }

    }
}
