using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WorkStationController.Core.Data;

namespace Visuals
{
    public class WorktableVisual
    {
        private Worktable worktable;
        private Size containerSize;
        public WorktableVisual(Worktable worktable, Size containerSize)
        {
            this.worktable = worktable;
            this.containerSize = containerSize;
        }

        public void Draw(DrawingContext drawingContext)
        {
            for (int i = 0; i < worktable.GridCount; i++)
            {
                DrawThisGrid(i, drawingContext);
            }
        }

        private void DrawThisGrid(int grid, DrawingContext drawingContext)
        {
            Point ptStart = worktable.FirstPinPosition;
            int offset = grid * Worktable.distanceBetweenAdjacentPins;
            int firstPinX = (int)ptStart.X + offset;
            int firstPinY = (int)ptStart.Y;
            DrawThisCell(firstPinX, firstPinY,worktable.FirstRowPinSize,drawingContext);
        }

        private double Convert2PixelXUnit(double x)
        {
            return x / worktable.Size.Width * containerSize.Width;
        }
        private double Convert2PixelYUnit(double y)
        {
            return y / worktable.Size.Height * containerSize.Height;
        }

        private void DrawThisCell(int pinX, int pinY, Size size, DrawingContext drawingContext)
        {
            double pinXPosPixel = Convert2PixelXUnit(pinX);
            double pinYPospixel = Convert2PixelXUnit(pinY);
            double pinWidthPixel = Convert2PixelXUnit(size.Width);
            double pinHeightpixel = Convert2PixelXUnit(size.Height);
            Brush brush = Brushes.Black;
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(Colors.Black), 1), 
                new Rect(new Point(pinXPosPixel,pinYPospixel),new Size(pinWidthPixel,pinHeightpixel)));
        }
    }
}
