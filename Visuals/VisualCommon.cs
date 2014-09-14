using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WorkStationController.Core.Data;
namespace Visuals
{
    public class VisualCommon
    {
        public static Size containerSize;
        private static double Convert2PixelXUnit(double x)
        {
            return x / Configurations.Instance.Worktable.Size.Width * containerSize.Width;
        }
        private static double Convert2PixelYUnit(double y)
        {
            return y / Configurations.Instance.Worktable.Size.Height * containerSize.Height;
        }

        public static void DrawRect(int x, int y, Size size, DrawingContext drawingContext, Color color)
        {
            double pinXPosPixel = VisualCommon.Convert2PixelXUnit(x);
            double pinYPospixel = VisualCommon.Convert2PixelXUnit(y);
            double pinWidthPixel = VisualCommon.Convert2PixelXUnit(size.Width);
            double pinHeightpixel = VisualCommon.Convert2PixelXUnit(size.Height);
            Brush brush = new SolidColorBrush(color);
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(color), 1),
                new Rect(new Point(pinXPosPixel, pinYPospixel), new Size(pinWidthPixel, pinHeightpixel)));
        }

    }
}
