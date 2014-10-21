using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    class LabwareUIElementFixedSize : UIElement
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
        public LabwareUIElementFixedSize(Labware labware,Size boundingSize)
        {
            this._labware = labware;
            _children.Add(CreateBorderVisual());
            UpdateContainerSize(boundingSize);
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
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var position = _labware.GetPosition(row, col);
                    //VisualCommon.DrawCircle(position, _labware.WellsInfo.WellRadius, drawingContext, _labware.BackgroundColor);
                }
            }
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
            Brush brush = new SolidColorBrush(color);
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(Colors.Black), 1), Rectangle);
        }

        internal void DrawCircle(Point ptCenter,double radius, Color color, DrawingContext drawingContext)
        {
            Brush brush = new SolidColorBrush(color);
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
