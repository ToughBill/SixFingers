using System;
using System.Collections.Generic;
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
        }

        public  void UpdateContainerSize(Size newSize)
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
            int wellIndex = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
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
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(Colors.Black), 1), ptVisual, sz.Width, sz.Height);
            drawingVisual.Index =  index;
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
            drawingVisual.Index = 0;
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
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(Colors.Black), 1), ptCenter, radius, radius);
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

        public class MyDrawingVisual : DrawingVisual
        {
            public int Index { get; set; }

        }
    }
}
