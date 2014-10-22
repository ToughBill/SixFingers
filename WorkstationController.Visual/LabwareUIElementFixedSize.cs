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

        private VisualCollection _children;
        private Size _boudingSize;
        private readonly Color defaultCircleColor = Colors.DarkGray;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="labware"></param>
        /// <param name="boundingSize"></param>
        public LabwareUIElementFixedSize(Labware labware,Size boundingSize)
        {
            this._labware = labware;
            UpdateContainerSize(boundingSize);
            _children = new VisualCollection(this);
            _children.Add(CreateBorderVisual());
            AddWellVisuals();
            this.MouseMove += LabwareUIElementFixedSize_MouseMove;
            this.MouseLeftButtonDown += LabwareUIElementFixedSize_MouseLeftButtonDown;
        }

        void LabwareUIElementFixedSize_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (MyDrawingVisual drawingVisual in _children)
            {
                if (drawingVisual.State == WellState.HighLight)
                {
                    ModifyState(drawingVisual, WellState.Normal);
                }
            }
            Point pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(this, null,
                new HitTestResultCallback(MyHitTestResultMouseMove),
                new PointHitTestParameters(pt));
        }

        /// <summary>
        /// return the selected wells, use List<> for backward compatible.
        /// </summary>
        public List<int> SelectedWellIDs
        {
            get
            {
                List<int> selected = new List<int>();
                foreach(MyDrawingVisual drawingVisual in _children)
                {
                    if(drawingVisual.State == WellState.Selected)
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
            Brush brush;
            switch(state)
            {
                case WellState.HighLight:
                    brush = Brushes.Yellow;
                    break;
                case WellState.Selected:
                    brush = Brushes.DarkGreen;
                    break;
                default:
                    brush = Brushes.White;
                    break;
            }
            DrawingContext drawingContext = drawingVisual.RenderOpen();
           

            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(defaultCircleColor), 1), drawingVisual.Position, drawingVisual.Size.Width, drawingVisual.Size.Height);
            if(state != WellState.Normal)
            {
                string sText = GetDescription(drawingVisual.ID);
                Point ptDesc = drawingVisual.Position;
                ptDesc.Y -= drawingVisual.Size.Height;
                ptDesc.X -= 1.2*drawingVisual.Size.Width;
                drawingContext.DrawText(new FormattedText(sText,
                             CultureInfo.GetCultureInfo("en-us"),
                             FlowDirection.LeftToRight,
                             new Typeface("Verdana"),
                             14 * _boudingSize.Width / 400, System.Windows.Media.Brushes.DarkBlue),
                             ptDesc);
             }
            drawingContext.Close();
            
        }

        private string GetDescription(int id)
        {
            int index = id - 1;
            int colIndex = index / _labware.WellsInfo.NumberOfWellsY;
            int rowID = id - colIndex * _labware.WellsInfo.NumberOfWellsY;
            int colID = colIndex + 1;
            return string.Format("{0}-{1}", colID, rowID);
        }

        void LabwareUIElementFixedSize_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (MyDrawingVisual drawingVisual in _children)
            {
                if (drawingVisual.State == WellState.Selected)
                {
                    ModifyState(drawingVisual,WellState.Normal);
                }
            }
            Point pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(this, null,
                new HitTestResultCallback(MyHitTestResultMouseDown),
                new PointHitTestParameters(pt));
        }

        private HitTestResultBehavior MyHitTestResultMouseDown(HitTestResult result)
        {
            return OnHit(result, WellState.Selected);
        }

        private HitTestResultBehavior OnHit(HitTestResult result, WellState wellState)
        {
            MyDrawingVisual hitVisual = (MyDrawingVisual)result.VisualHit;
            if (hitVisual == null || hitVisual.ID == 0)
                return HitTestResultBehavior.Continue;

            //need to fill
            bool canNOToverwrite = (hitVisual.State == WellState.Selected && wellState == WellState.HighLight);
            if(!canNOToverwrite)
                ModifyState(hitVisual, wellState);
            return HitTestResultBehavior.Continue;
        }

        private HitTestResultBehavior MyHitTestResultMouseMove(HitTestResult result)
        {
            return OnHit(result, WellState.HighLight);
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
            int cols = _labware.WellsInfo.NumberOfWellsX;
            int rows = _labware.WellsInfo.NumberOfWellsY;
            int wellIndex = 1;
            
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var position = _labware.GetPosition(row, col);
                    _children.Add(CreateWellVisual(position, wellIndex++));
                }
            }
        }

        private MyDrawingVisual CreateWellVisual(Point position, int index)
        {
            MyDrawingVisual drawingVisual = new MyDrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            int radius = _labware.WellsInfo.WellRadius;
            Brush brush = Brushes.White;
            Size labwareSize = new Size(_labware.Dimension.XLength,_labware.Dimension.YLength);
            Point ptVisual = Physical2Visual(position, labwareSize, _boudingSize);
            Size sz = new Size(radius, radius);
            sz = Physical2Visual(sz, labwareSize, _boudingSize);
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(defaultCircleColor), 1), ptVisual, sz.Width, sz.Height);
            drawingVisual.ID =  index;
            drawingVisual.Position = ptVisual;
            drawingVisual.Size = sz;
            drawingContext.Close();
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

        private Visual CreateBorderVisual()
        {
            MyDrawingVisual drawingVisual = new MyDrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            DrawRect( new Rect(_boudingSize), Colors.Black, drawingContext);
            drawingVisual.ID = 0;
            drawingContext.Close();
            return drawingVisual;
        }

        private void DrawRect(Rect Rectangle, Color color, DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(color), 1), Rectangle);
        }

        internal void DrawCircle(Point ptCenter,double radius, Color color, DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(color);
            if (color == Colors.White)
                brush = null;
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(defaultCircleColor), 1), ptCenter, radius, radius);
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

        /// <summary>
        /// drawing visual with more information
        /// </summary>
        public class MyDrawingVisual : DrawingVisual
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
