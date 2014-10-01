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

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// response to user's actions and control the render of UI
    /// </summary>
    public class UIMovementsController : INotifyPropertyChanged
    {
        private bool isDragging = false;
        private System.Windows.Controls.Grid myCanvas;
        private Point ptStartDrag;
        private UIElement _selectedUIElement;
        private UIElement _newUIElement;

        /// <summary>
        /// new UI element introduced from somewhere, nomarlly from listbox
        /// </summary>
        public UIElement NewUIElement
        {
            get
            {
                return _newUIElement;
            }
            set
            {
                _newUIElement = value;
            }
        }
        
        private bool enableMouseMove = true;

        /// <summary>
        /// nothing to say
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        

        /// <summary>
        /// selected UI element
        /// </summary>
        public UIElement SelectedElement
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
            this.myCanvas = grid;
            myCanvas.MouseLeftButtonDown += myCanvas_MouseLeftButtonDown;
            myCanvas.MouseLeftButtonUp += myCanvas_MouseLeftButtonUp;
            myCanvas.MouseMove += myCanvas_MouseMove;
        }

        void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;
            if (!enableMouseMove)
                return;
            Point ptClick = e.GetPosition(myCanvas);
            if (ptClick.X < 0 || ptClick.X > myCanvas.ActualWidth)
                return;

            if (ptClick.Y < 0 || ptClick.Y > myCanvas.ActualHeight)
                return;

            //double actualWidth = 0;
            //double actualHeight = 0;
            UIElement workingElement = NewUIElement == null ? SelectedElement : NewUIElement;
            if (workingElement == null)
                return;
            ptClick.Offset(-ptStartDrag.X, -ptStartDrag.Y);


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

            if (NewUIElement != null)
            {
                myCanvas.Children.Add(NewUIElement);
                myCanvas.InvalidateVisual();
                SelectedElement = NewUIElement;
                NewUIElement = null;
                return;
            }

            if (SelectedElement == null)
                return;

            ((LabwareUIElement)SelectedElement).Update();
            SelectedElement.RenderTransform = new TranslateTransform(ptClick.X, ptClick.Y);
            
        }

        void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myCanvas.ReleaseMouseCapture();
            
        }

        void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PreventMouseMove();
            _selectedUIElement = FindSelectedUIElement(e.GetPosition(myCanvas));
            if (_selectedUIElement != null)
            {
                myCanvas.CaptureMouse();
            }
            AllowMouseMove();
        }

        void myCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            myCanvas.ReleaseMouseCapture();
        }

        /// <summary>
        /// whether we are dragging the UI element from the listbox
        /// </summary>
        public bool IsDragging
        {
            set
            {
                isDragging = value;
                if (isDragging)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    SelectedElement = null;
                }
            }
        }


        //only find LabwareUIElement & CarrierUIElement
        private UIElement FindSelectedUIElement(Point pt)
        {
            HitTestResult result = VisualTreeHelper.HitTest(myCanvas, pt);
            if (result == null)
            {
                return null;
            }
            ptStartDrag = pt;
            var uiElement = VisualCommon.FindParent<LabwareUIElement>(result.VisualHit);
            if (uiElement != null)
                return uiElement;
            return VisualCommon.FindParent<CarrierUIElement>(result.VisualHit);
        }


        private void AllowMouseMove()
        {
            enableMouseMove = true;
        }

        private void PreventMouseMove()
        {
            enableMouseMove = false;
        }

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
