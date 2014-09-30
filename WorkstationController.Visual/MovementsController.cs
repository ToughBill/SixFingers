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
    class MovementsController : INotifyPropertyChanged
    {
        private bool isDragging = false;
        private System.Windows.Controls.Grid myCanvas;
        private Point ptStartDrag;
        private UIElement _selectedElement;
        private UIElement _newLabwareElement;


        public UIElement NewLabwareElement
        {
            get
            {
                return _newLabwareElement;
            }
            set
            {
                _newLabwareElement = value;
            }
        }
        
        private bool enableMouseMove = true;
        public event PropertyChangedEventHandler PropertyChanged;
        private string selName;


        public UIElement SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
        }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        public MovementsController(System.Windows.Controls.Grid grid)
        {
            // TODO: Complete member initialization
            this.myCanvas = grid;
        }


        public bool IsDragging
        {
            set
            {
                isDragging = value;
                RemoveSelectionAdorner();
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

        internal void CanvasLeftButtonUp(MouseButtonEventArgs e)
        {
            myCanvas.ReleaseMouseCapture();
        }
 
        internal void CanvasLeftButtonDown(MouseButtonEventArgs e)
        {
            PreventMouseMove();
            _selectedElement = FindSelectedUIElement(e.GetPosition(myCanvas));
            if(_selectedElement != null)
            {
                myCanvas.CaptureMouse();
                AddSelectionAdorner(_selectedElement);
            }
            AllowMouseMove();

        }

        private UIElement FindSelectedUIElement(Point pt)
        {
            HitTestResult result = VisualTreeHelper.HitTest(myCanvas, pt);
            if (result == null)
            {
                return null;
            }
            ptStartDrag = pt;
            return VisualCommon.FindParent<UIElement>(result.VisualHit);
        }


        private void AllowMouseMove()
        {
            enableMouseMove = true;
        }

        private void PreventMouseMove()
        {
            enableMouseMove = false;
        }

        private void RemoveSelectionAdorner()
        {
            if (SelectedElement == null)
                return;

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(SelectedElement);
            Adorner[] toRemoveArray = adornerLayer.GetAdorners(SelectedElement);
            Adorner toRemove;
            if (toRemoveArray != null)
            {
                toRemove = toRemoveArray[0];
                adornerLayer.Remove(toRemove);
            }

        }
        private void AddSelectionAdorner(UIElement uiElement)
        {
            RemoveSelectionAdorner();
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
            adornerLayer.Add(new BorderAdorner(uiElement));
        }

        internal void MoveMouse(MouseEventArgs e)
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

            double actualWidth = 0;
            double actualHeight = 0;
            UIElement workingElement = NewLabwareElement == null ? SelectedElement : NewLabwareElement;
            if (workingElement == null)
                return;
            ptClick.Offset(-ptStartDrag.X, -ptStartDrag.Y);
            
            Rect rc;// = workingElement.Labware.labBase.Rectangle;
            if(workingElement is LabwareUIElement)
            {
//                rc.Width = ((LabwareUIElement)workingElement)
            }
            //actualWidth = rc.Width;
            //actualHeight = rc.Height;

            //if (ptClick.X < 0)
            //    ptClick.X = 0;
            //if (ptClick.X + actualWidth > myCanvas.ActualWidth)
            //    ptClick.X = myCanvas.ActualWidth - actualWidth;
            //if (ptClick.Y < 0)
            //    ptClick.Y = 0;
            //if (ptClick.Y + actualHeight > myCanvas.ActualHeight)
            //    ptClick.Y = myCanvas.ActualHeight - actualHeight;


            if (NewLabwareElement != null)
            {
                myCanvas.Children.Add(NewLabwareElement);
                myCanvas.InvalidateVisual();
                SelectedElement = NewLabwareElement;
                NewLabwareElement = null;
                return;
            }

            if (SelectedElement == null)
                return;

            SelectedElement.RenderTransform = new TranslateTransform(ptClick.X, ptClick.Y);
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
