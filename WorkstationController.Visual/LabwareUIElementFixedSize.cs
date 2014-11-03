using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    public class LabwareUIElementFixedSize : UIElement
    {

        /// <summary>
        /// no well is selected;
        /// </summary>
        Labware _labware;
        List<MyDrawingVisual> wellVisuals;
        private Size _boudingSize;
        private readonly Color defaultCircleColor = Colors.DarkGray;
        private Rect _tightBoundingRect;
        private SingleSelection _singleSelection;
        private MultiSelection _multiSelection;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="labware"></param>
        /// <param name="boundingSize"></param>
        public LabwareUIElementFixedSize(Labware labware,Size boundingSize)
        {
            this._labware = labware;
            UpdateContainerSize(boundingSize);
            AddWellVisuals();
            _singleSelection = new SingleSelection(wellVisuals, _tightBoundingRect, _labware);
            _multiSelection = new MultiSelection(wellVisuals);
            //this.MouseMove += LabwareUIElementFixedSize_MouseMove;
            this.MouseLeftButtonDown += LabwareUIElementFixedSize_MouseLeftButtonDown;
            this.MouseMove += LabwareUIElementFixedSize_MouseMove;
            this.MouseUp += LabwareUIElementFixedSize_MouseUp;
        }
        void LabwareUIElementFixedSize_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);
            _multiSelection.OnLeftButtonDown(pt);
            _singleSelection.OnLeftButtonDown(pt);
            InvalidateVisual();
        }

        void LabwareUIElementFixedSize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _multiSelection.OnLeftButtonUp(e.GetPosition(this));
            InvalidateVisual();
        }

        void LabwareUIElementFixedSize_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);
            _multiSelection.OnMouseMove(pt);
            _singleSelection.OnMouseMove(pt);
            InvalidateVisual();
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawBorder(drawingContext);
            if(_multiSelection.IsValid)
            {
                DrawRect(_multiSelection.Rect, Colors.Blue, drawingContext);
            }
            foreach(MyDrawingVisual drawingVisual in wellVisuals)
            {
                DrawCircle(drawingVisual.Position,
                    drawingVisual.Size.Width,
                    GetStateColor(drawingVisual.State),
                    drawingContext);

                if (drawingVisual.State != WellState.Normal)
                {
                    string sText = GetDescription(drawingVisual.ID);
                    Point ptDesc = drawingVisual.Position;
                    ptDesc.Y -= drawingVisual.Size.Height;
                    ptDesc.X -= 1.2 * drawingVisual.Size.Width;
                    drawingContext.DrawText(new FormattedText(sText,
                                 CultureInfo.GetCultureInfo("en-us"),
                                 FlowDirection.LeftToRight,
                                 new Typeface("Verdana"),
                                 14 * _boudingSize.Width / 400, System.Windows.Media.Brushes.DarkBlue),
                                 ptDesc);
                }
            }
        }

        /// <summary>
        /// return the selected wells, use List<> for backward compatible.
        /// </summary>
        public List<int> SelectedWellIDs
        {
            get
            {
                List<int> selected = new List<int>();
                foreach (MyDrawingVisual drawingVisual in wellVisuals)
                {
                    if (drawingVisual.State == WellState.Selected)
                    {
                        selected.Add(drawingVisual.ID);
                    }
                }
                return selected;
            }
        }
       

        private Color GetStateColor(WellState state)
        {
            Color color;
            switch (state)
            {
                case WellState.HighLight:
                    color = Colors.Yellow;
                    break;
                case WellState.Selected:
                    color = Colors.LightGreen;
                    break;
                default:
                    color = Colors.White;
                    break;
            }
            return color;
        }
        private string GetDescription(int id)
        {
            int index = id - 1;
            int colIndex = index / _labware.WellsInfo.NumberOfWellsY;
            int rowID = id - colIndex * _labware.WellsInfo.NumberOfWellsY;
            int colID = colIndex + 1;
            return string.Format("{0}-{1}", colID, rowID);
        }

        private void UpdateContainerSize(Size newSize)
        {
            Size wareSZ = new Size(_labware.Dimension.XLength,_labware.Dimension.YLength);
            _boudingSize = newSize;
            double screenRatio = newSize.Width / newSize.Height;
            double realRatio = wareSZ.Width / wareSZ.Height;
            if (realRatio > screenRatio)//x方向占满
            {
                _boudingSize.Height = _boudingSize.Height / (realRatio / screenRatio);
            }
            else //y方向占满
            {
                _boudingSize.Width = _boudingSize.Width / (screenRatio / realRatio);
            }
        }
        private void AddWellVisuals()
        {
            wellVisuals = new List<MyDrawingVisual>();
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            int wellID = 1;
            int total = cols * rows;
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var position = _labware.GetPosition(row, col);
                    wellVisuals.Add(CreateWellVisual(position, wellID++,total));
                }
            }
        }

        private MyDrawingVisual CreateWellVisual(Point position, int wellID, int totalCnt)
        {
            MyDrawingVisual drawingVisual = new MyDrawingVisual();
            int radius = _labware.WellsInfo.WellRadius;
            Brush brush = Brushes.White;
            Size labwareSize = new Size(_labware.Dimension.XLength,_labware.Dimension.YLength);
            Point ptVisual = Physical2Visual(position, labwareSize, _boudingSize);
            Size sz = new Size(radius, radius);
            sz = Physical2Visual(sz, labwareSize, _boudingSize);
            drawingVisual.ID =  wellID;
            drawingVisual.Position = ptVisual;
            drawingVisual.Size = sz;
            if (wellID == 1)
            {
                Point ptTmp = ptVisual;
                ptTmp -= new Vector(sz.Width, sz.Height);
                _tightBoundingRect.X = ptTmp.X;
                _tightBoundingRect.Y = ptTmp.Y;
            }
            if (wellID == totalCnt)
            {
                _tightBoundingRect.Width = ptVisual.X - _tightBoundingRect.X + sz.Width;
                _tightBoundingRect.Height = ptVisual.Y - _tightBoundingRect.Y + sz.Height;
            }

            return drawingVisual;
        }

        private Size Physical2Visual(Size sz, Size labwareSize, Size _boudingSize)
        {
            Size szVisual = new Size();
            szVisual.Width = sz.Width / labwareSize.Width * _boudingSize.Width;
            szVisual.Height = sz.Height / labwareSize.Height * _boudingSize.Height;
            return szVisual;
        }

        private Point Physical2Visual(Point position, Size labwareSize, Size _boudingSize)
        {
            Point ptVisual = new Point();
            ptVisual.X = position.X / labwareSize.Width * _boudingSize.Width;
            ptVisual.Y = position.Y / labwareSize.Height * _boudingSize.Height;
            return ptVisual;
        }

        private void DrawBorder(DrawingContext dc)
        {
            DrawRect( new Rect(_boudingSize), Colors.Black, dc);
        }

        private void DrawRect(Rect Rectangle, Color color, DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(Color.FromArgb(20, 200, 200, 200));
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(color), 1), Rectangle);
        }

        internal void DrawCircle(Point ptCenter,double radius, Color color, DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(color);
            if (color == Colors.White)
                brush = null;
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(defaultCircleColor), 1), ptCenter, radius, radius);
        }
    }

    /// <summary>
    /// each well's state
    /// </summary>
    public enum WellState
    {
        Normal = 0,
        HighLight,
        Selected
    }
    /// <summary>
    /// drawing visual with more information
    /// </summary>
    public class MyDrawingVisual
    {
        public int ID { get; set; }
        public WellState State { get; set; }
        public Point Position { get; set; }
        public Size Size { get; set; }
    }


    class SingleSelection
    {
        MyDrawingVisual _selectedWell;
        List<MyDrawingVisual> _allWellVisuals;
        Rect _tightBoundingRect;
        Labware _labware;
        bool bValid = false;
        public SingleSelection(List<MyDrawingVisual> allWellVisuals,Rect rcBoundary, Labware labware)
        {
            _allWellVisuals = allWellVisuals;
            _labware = labware;
            _tightBoundingRect = rcBoundary;
        }

        bool IsCtrlPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        public int GetFocusID(Point pt)
        {
            if (!_tightBoundingRect.Contains(pt))
                return -1;
            double unitX = _tightBoundingRect.Width / _labware.WellsInfo.NumberOfWellsX;
            double unitY = _tightBoundingRect.Height / _labware.WellsInfo.NumberOfWellsY;
            int colID = (int)Math.Ceiling((pt.X - _tightBoundingRect.X) / unitX);
            int rowID = (int)Math.Ceiling((pt.Y - _tightBoundingRect.Y) / unitY);
            return (colID - 1) * _labware.WellsInfo.NumberOfWellsY + rowID;
        }

        public void OnLeftButtonDown(Point pt)
        {
            if (_selectedWell != null)
                _selectedWell.State = WellState.Normal;
            if (IsCtrlPressed())
            {
                bValid = false;
                return;
            }
            int focusID = GetFocusID(pt);
            foreach (MyDrawingVisual drawingVisual in _allWellVisuals)
            {
                if (drawingVisual.State == WellState.Selected)
                {
                    drawingVisual.State = WellState.Normal;
                }
            }

            foreach (MyDrawingVisual drawingVisual in _allWellVisuals)
            {
                if (drawingVisual.ID == focusID)
                {
                    drawingVisual.State = WellState.Selected;
                    break;
                }
            }
        }


        internal void OnMouseMove(Point pt)
        {
            if (IsCtrlPressed())
            {
                //SetAll2Default();
                return;
            }
            int focusID = GetFocusID(pt);
            foreach (MyDrawingVisual drawingVisual in _allWellVisuals)
            {
                if (drawingVisual.State == WellState.Selected) //don't change the selected one.
                    continue;
                WellState state = drawingVisual.ID == focusID ? WellState.HighLight : WellState.Normal;
                drawingVisual.State = state;
            }
        }

        private void SetAll2Default()
        {
            foreach (MyDrawingVisual drawingVisual in _allWellVisuals)
            {
                if (drawingVisual.State == WellState.Normal)
                    continue;
                drawingVisual.State = WellState.Normal;
            }
        }
    }

    class MultiSelection
    {
        private bool _bValid = false;
        private Point _ptStart;
        private Point _ptEnd;
        
        List<MyDrawingVisual> _allWellVisuals;

        public MultiSelection( List<MyDrawingVisual> allWellVisuals)
        {
            _allWellVisuals = allWellVisuals;
        }

        bool IsWellSelected(Point ptWell)
        {
            Rect rc = new Rect(_ptStart, _ptEnd);
            return rc.Contains(ptWell);
        }

        bool IsCtrlPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        private void SetAll2Default()
        {
            foreach (MyDrawingVisual drawingVisual in _allWellVisuals)
            {
                if (drawingVisual.State == WellState.Normal)
                    continue;
                drawingVisual.State = WellState.Normal;
            }
        }

        public void OnLeftButtonDown(Point pt)
        {
            if (IsCtrlPressed())
            {
                _ptStart = pt;
                _ptEnd = _ptStart;
                _bValid = true;
                SetAll2Default();
            }
            else
            {
                _bValid = false;
            }
        }

        public void OnMouseMove(Point point)
        {
            if(!_bValid)
               return ;
            _ptEnd = point;
        }

        internal void OnLeftButtonUp(Point point)
        {
            if(!_bValid)
                return;

            _bValid = false;
            Rect rc = new Rect(_ptStart, _ptEnd);
            foreach(var wellVisual in _allWellVisuals)
            {
                if(rc.Contains(wellVisual.Position))
                {
                    wellVisual.State = WellState.Selected;
                }
            }

        }

        public bool IsValid
        { 
            get 
            { 
                return _bValid; 
            }
        }

        public Rect Rect
        { 
            get
            {
                return new Rect(_ptStart, _ptEnd);
            }
        
        }
    }
}
