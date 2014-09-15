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
        /// <summary>
        /// The container size
        /// </summary>
        public static Size containerSize;

        /// <summary>
        /// Draw a rectangle at specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="drawingContext"></param>
        /// <param name="color"></param>
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

        /// <summary>
        /// Convert the X coordination on worktable to the corresponding X position on container
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Convert2PixelXUnit(double x)
        {
            return x / Configurations.Instance.Worktable.Size.Width * containerSize.Width;
        }

        /// <summary>
        /// Convert the Y coordination on worktable to the corresponding Y position on container
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double Convert2PixelYUnit(double y)
        {
            return y / Configurations.Instance.Worktable.Size.Height * containerSize.Height;
        }
    }
}
