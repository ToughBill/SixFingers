using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
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
        private Point curMouse;
        private Size _boudingSize;
        private readonly Color defaultCircleColor = Colors.DarkGray;
        private Rect tightBoundingRect;
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
            //this.MouseMove += LabwareUIElementFixedSize_MouseMove;
            this.MouseLeftButtonDown += LabwareUIElementFixedSize_MouseLeftButtonDown;
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawBorder(drawingContext);
            //DrawRect(tightBoundingRect, Colors.LightGreen, drawingContext);
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

        public void UpdateMousePosition(System.Windows.Input.MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);
            curMouse = pt;
            int focusID = GetFocusID(pt);
            foreach (MyDrawingVisual drawingVisual in wellVisuals)
            {
                if (drawingVisual.State == WellState.Selected) //don't change the selected one.
                    continue;
                WellState state = drawingVisual.ID == focusID ? WellState.HighLight : WellState.Normal;
                ModifyState(drawingVisual, state);
            }
            InvalidateVisual();
        }

        void LabwareUIElementFixedSize_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);
            int focusID = GetFocusID(pt);
            
            foreach (MyDrawingVisual drawingVisual in wellVisuals)
            {
                if (drawingVisual.State == WellState.Selected)
                {
                    ModifyState(drawingVisual, WellState.Normal);
                }
            }
          
            foreach (MyDrawingVisual drawingVisual in wellVisuals)
            {
                if (drawingVisual.ID == focusID)
                {
                    ModifyState(drawingVisual, WellState.Selected);
                    break;
                }
            }
            InvalidateVisual();
        }


        private int GetFocusID(Point pt)
        {
            if (!tightBoundingRect.Contains(pt))
                return -1;
            double unitX = tightBoundingRect.Width / _labware.WellsInfo.NumberOfWellsX;
            double unitY = tightBoundingRect.Height / _labware.WellsInfo.NumberOfWellsY;
            int colID = (int)Math.Ceiling((pt.X - tightBoundingRect.X) / unitX);
            int rowID = (int)Math.Ceiling((pt.Y - tightBoundingRect.Y) / unitY);
            return (colID-1) * _labware.WellsInfo.NumberOfWellsY + rowID;
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
        private void ModifyState(MyDrawingVisual drawingVisual, WellState state)
        {
            drawingVisual.State = state;
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
                tightBoundingRect.X = ptTmp.X;
                tightBoundingRect.Y = ptTmp.Y;
            }
            if (wellID == totalCnt)
            {
                tightBoundingRect.Width = ptVisual.X - tightBoundingRect.X + sz.Width;
                tightBoundingRect.Height = ptVisual.Y - tightBoundingRect.Y + sz.Height;
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

        /// <summary>
        /// each well's state
        /// </summary>
        public enum WellState
        {
            Normal = 0,
            HighLight,
            Selected
        }

    
    }
}
