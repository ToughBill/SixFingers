using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    /// <summary>
    /// 
    /// </summary>
    public class VisualCommon
    {
        public static Size containerSize;

        public static double Convert2PixelXUnit(double x)
        {
            return x / Configurations.Instance.Worktable.Size.Width * containerSize.Width;
        }
        public static double Convert2PixelYUnit(double y)
        {
            return y / Configurations.Instance.Worktable.Size.Height * containerSize.Height;
        }


        public static T FindParent<T>(DependencyObject from)
where T : class
        {
            T result = null;
            DependencyObject parent = VisualTreeHelper.GetParent(from);

            if (parent is T)
                result = parent as T;
            else if (parent != null)
                result = FindParent<T>(parent);
            return result;
        }


        public static void DrawRect(int x, int y, Size size, DrawingContext drawingContext, Color color)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(x);
            double yPixel = VisualCommon.Convert2PixelYUnit(y);
            double wPixel = VisualCommon.Convert2PixelXUnit(size.Width);
            double hPixel = VisualCommon.Convert2PixelYUnit(size.Height);
            Brush brush = new SolidColorBrush(color);
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(color), 1),
                new Rect(new Point(xPixel, yPixel), new Size(wPixel, hPixel)));
        }


        internal static void DrawCircle(Point position, int radius, DrawingContext drawingContext, Color color)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(position.X);
            double yPixel = VisualCommon.Convert2PixelYUnit(position.Y);
            double rXPixel = VisualCommon.Convert2PixelXUnit(radius);
            double rYPixel = VisualCommon.Convert2PixelYUnit(radius);
            Brush brush = new SolidColorBrush(color);
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(color), 1), new Point(xPixel, yPixel), rXPixel, rYPixel);
        }
    }
}
