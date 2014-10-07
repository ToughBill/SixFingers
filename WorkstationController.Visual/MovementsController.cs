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

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// response to user's actions and control the render of UI
    /// </summary>
    public class UIMovementsController : INotifyPropertyChanged
    {
        //private bool isDragging = false;
        private System.Windows.Controls.Grid _myCanvas;
        private Point _ptClick;
        private BasewareUIElement _selectedUIElement;
        private BasewareUIElement _uiElementCandidate;
        private bool enableMouseMove = true;

        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
      
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
                OnPropertyChanged("BasewareUIElement");
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
                OnPropertyChanged("SelectedElement");
            }
        }

        /// <summary>
        /// Create the OnPropertyChanged method to raise the event 
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
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

            if(_selectedUIElement is CarrierUIElement)
            {
                MountTheCarrier((CarrierUIElement)_selectedUIElement,e.GetPosition(_myCanvas));
            }
            else
            {
                MountTheLabware((LabwareUIElement)_selectedUIElement, e.GetPosition(_myCanvas));
            }
            //_selectedUIElement.RenderTransform = new TranslateTransform(0, 0);
            _selectedUIElement = null;
        }

        //mount labware & carrier need to be rewrite for simple.
        private void MountTheLabware(LabwareUIElement labwareUIElement, Point position)
        {
            int grid = VisualCommon.FindCorrespondingGrid(position.X);
            //bool suitableCarrier = Configurations.Instance.L

            _selectedUIElement.Selected = false;
            
        }

        private void MountTheCarrier(CarrierUIElement _selectedUIElement,Point position)
        {
            int grid = VisualCommon.FindCorrespondingGrid(position.X);
            _selectedUIElement.Selected = false;
            _selectedUIElement.Grid = grid;
        }

        void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;
            if (!enableMouseMove)
                return;
            
            
            Point ptClick = e.GetPosition(_myCanvas);
            if (ptClick.X < 0 || ptClick.X > _myCanvas.ActualWidth)
                return;

            if (ptClick.Y < 0 || ptClick.Y > _myCanvas.ActualHeight)
                return;
            
            bool hasUIElement2Operate = _uiElementCandidate != null || _selectedUIElement != null;
            if (!hasUIElement2Operate)
                return;
            
            
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
            UpdateSelectedElement(ptClick);
            
        }

        //public void RemoveCurrentSelectFlag()
        //{
        //    for( int i = 0; i< _myCanvas.Children.Count; i++)
        //    {
        //        var baseUIElement = (BasewareUIElement)_myCanvas.Children[i];
        //        baseUIElement.Selected = false;
        //    }
        //}

        private void UpdateSelectedElement(Point ptCurrent)
        {
            _selectedUIElement.SetDragPosition(ptCurrent);
            _selectedUIElement.InvalidateVisual();
            //SelectedElement.RenderTransform = new TranslateTransform(ptStart.X, ptStart.Y);
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


        //private void AllowMouseMove()
        //{
        //    Debug.WriteLine("Allow mouse move!");
        //    enableMouseMove = true;
        //}

        //private void PreventMouseMove()
        //{
        //    Debug.WriteLine("Prevent mouse move!");
        //    enableMouseMove = false;
        //}

        //public bool MoveSelectedElementTo(string sXOffSet, string sYOffSet, ref string sErrMsg)
        //{
        //    double xOffSet;
        //    double yOffSet;

        //    bool bok = CheckValueInRange("X偏移", sXOffSet, 0, myCanvas.ActualWidth, out xOffSet, ref sErrMsg);
        //    if (!bok)
        //        return false;

        //    bok = CheckValueInRange("Y偏移", sYOffSet, 0, myCanvas.ActualHeight, out yOffSet, ref sErrMsg);
        //    if (!bok)
        //        return false;

        //    if (_selectedElement == null)
        //    {
        //        sErrMsg = "没有器件被选中！";
        //        return false;
        //    }


        //    _selectedElement.TopLeft = new Point(AxisAdorner.TranslateMM2PtX(xOffSet), AxisAdorner.TranslateMM2PtY(yOffSet));
        //    return true;
        //}

        //private bool CheckValueInRange(string testValName, string sVal, double min, double max, out double val, ref string sErrMsg)
        //{
        //    bool bok = double.TryParse(sVal, out val);
        //    if (!bok)
        //    {
        //        sErrMsg = string.Format("{0}必须为数字！", testValName);
        //        return false;
        //    }
        //    if (val < min || val > max)
        //    {
        //        sErrMsg = string.Format("{0}必须在{1}到{2}之间！", val, min, max);
        //        return false;
        //    }
        //    return true;
        //}


    }
}
