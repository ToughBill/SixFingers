using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement.Uitility
{
    /// <summary>
    /// render related
    /// </summary>
    public class VisualCommon
    {
        private const double worktableOccupiesRatio = 0.8;
        /// <summary>
        /// container's size;
        /// </summary>
        public static Size containerSize;

        /// <summary>
        /// from physical length to pixel units
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Convert2PixelXUnit(double x)
        {
            double percent = x / Configurations.Instance.Worktable.Size.Width;
            return worktableOccupiesRatio * percent * containerSize.Width;
        }
        private static Size GetWholeTable()
        {
            double width = worktableOccupiesRatio * containerSize.Width;
            double height = worktableOccupiesRatio * containerSize.Height;
            return new Size(width, height);
        }

        /// <summary>
        /// same to previous
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Convert2PixelYUnit(double y)
        {
            double percent = y / Configurations.Instance.Worktable.Size.Height;
            return worktableOccupiesRatio * percent * containerSize.Height;
        }

        /// <summary>
        /// convert pixel on UI to physical position.
        /// </summary>
        /// <param name="xPixel"></param>
        /// <param name="yPixel"></param>
        /// <returns></returns>
        public static Point Convert2PhysicalXY(double xPixel,double yPixel)
        {
            double percentY = (yPixel - GetYShift()) / (worktableOccupiesRatio * containerSize.Height);
            double percentX = (xPixel-GetXShift()) / (worktableOccupiesRatio * containerSize.Width);
            return new Point( Configurations.Instance.Worktable.Size.Width * percentX,
                              Configurations.Instance.Worktable.Size.Height * percentY);
        }

        /// <summary>
        /// find the parent visual which type is T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
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
        /// <summary>
        /// as the name describes
        /// </summary>
        /// <param name="newSize"></param>
        public static void UpdateContainerSize(Size newSize)
        {
            Size worktableSize = Configurations.Instance.Worktable.Size;
            containerSize = newSize;
            double screenRatio = newSize.Width / newSize.Height;
            double realRatio = worktableSize.Width / worktableSize.Height;
            if (realRatio > screenRatio)//x方向占满
            {
                containerSize.Height = containerSize.Height / (realRatio / screenRatio);
            }
            else //y方向占满
            {
                containerSize.Width = containerSize.Width / (screenRatio / realRatio);
            }
        }

        /// <summary>
        /// redraw every UI elements
        /// </summary>
        /// <param name="uIElementCollection"></param>
        public static void UpdateVisuals(UIElementCollection uIElementCollection)
        {
            foreach (UIElement uiElement in uIElementCollection)
            {
                if (!(uiElement is BasewareUIElement))
                    continue;
                BasewareUIElement basewareUIElement = (BasewareUIElement)uiElement;
                if(basewareUIElement != null)
                    basewareUIElement.InvalidateVisual();
            }
        }

        /// <summary>
        /// from physical to visual
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rect Physic2Visual(int x, int y, Size size)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(x);
            double yPixel = VisualCommon.Convert2PixelYUnit(y);
            xPixel += GetXShift();
            yPixel += GetYShift();
            double wPixel = VisualCommon.Convert2PixelXUnit(size.Width);
            double hPixel = VisualCommon.Convert2PixelYUnit(size.Height);
            return new Rect(new Point(xPixel, yPixel), new Size(wPixel, hPixel));
        }

        /// <summary>
        /// most left position
        /// </summary>
        public static double MostLeft
        {
            get
            {
                return GetXShift() + VisualCommon.Convert2PixelXUnit(Configurations.Instance.Worktable.FirstPinPosition.X);
            }
        }

        /// <summary>
        /// from physical to visual 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Size Physic2Visual(Size size)
        {
            double wPixel = VisualCommon.Convert2PixelXUnit(size.Width);
            double hPixel = VisualCommon.Convert2PixelYUnit(size.Height);
            return new Size(wPixel, hPixel);
        }

        /// <summary>
        /// draw rect
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="drawingContext"></param>
        /// <param name="color"></param>
        /// <param name="brush"></param>
        public static void DrawRect(int x, int y, Size size, DrawingContext drawingContext, Color color, Brush brush = null, int thickness = 1)
        {
            Rect rc = Physic2Visual(x,y,size);
            if (brush == null)
                brush = Brushes.Transparent;
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(color), thickness), rc);
        }

     
        /// <summary>
        /// draw solid rect
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="drawingContext"></param>
        /// <param name="color"></param>
        public static void DrawSolidRect(int x, int y, Size size, DrawingContext drawingContext, Color color)
        {
            DrawRect(x, y, size, drawingContext, color, new SolidColorBrush(color));
        }

        /// <summary>
        /// draw circle
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="drawingContext"></param>
        /// <param name="color"></param>
        public static void DrawCircle(Point position, int radius, DrawingContext drawingContext, Color color)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(position.X);
            double yPixel = VisualCommon.Convert2PixelYUnit(position.Y);
            xPixel += GetXShift();
            yPixel += GetYShift();
            double rXPixel = VisualCommon.Convert2PixelXUnit(radius);
            double rYPixel = VisualCommon.Convert2PixelYUnit(radius);
            Brush brush = new SolidColorBrush(color);
            drawingContext.DrawEllipse(null, new Pen(new SolidColorBrush(color), 1), new Point(xPixel, yPixel), rXPixel, rYPixel);
        }

        private static double GetXShift()
        {
            return 0.1 * containerSize.Width;
        }

        private static double GetYShift()
        {
            return 0.1 * containerSize.Height;
        }

        internal static void DrawGridNumber(int grid, int firstPinX, DrawingContext dc)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(firstPinX) + GetXShift()*0.95;
            //when height is 400 use font 10
            
            double yPixel = containerSize.Height *0.81;
             dc.DrawText( new FormattedText((grid + 1).ToString(),
                          CultureInfo.GetCultureInfo("en-us"),
                          FlowDirection.LeftToRight,
                          new Typeface("Verdana"),
                          10 * containerSize.Height / 400, System.Windows.Media.Brushes.DarkBlue),
                          new System.Windows.Point(xPixel, yPixel));
        }

        internal static int FindCorrespondingGrid(double xPixelOnCanvas)
        {
            double unitXPixels = VisualCommon.Convert2PixelXUnit(Worktable.DistanceBetweenAdjacentPins);
            double startXmm = Configurations.Instance.Worktable.FirstPinPosition.X;
            double xPixelsOffset = VisualCommon.Convert2PixelXUnit(startXmm);
            int mapGrid =  (int)(Math.Ceiling((xPixelOnCanvas - GetXShift() - xPixelsOffset) / unitXPixels + 0.5));
            return mapGrid;

        }

    

        internal static void DrawText(Point pt, 
                                      string text,
                                      DrawingContext drawingContext)
                                      
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(pt.X) + GetXShift();
            double yPixel = VisualCommon.Convert2PixelYUnit(pt.Y) + GetYShift();
            Point ptVisual = new Point(xPixel, yPixel);
            double fontSize = 10 * containerSize.Height / 400;
            
            var formattedText = new FormattedText(text,
                         CultureInfo.GetCultureInfo("en-us"),
                         FlowDirection.LeftToRight,
                         new Typeface("Verdana"),
                         fontSize, System.Windows.Media.Brushes.DarkBlue);

            drawingContext.DrawText(formattedText,ptVisual);
        }

        internal static void DrawLine(Point ptStart, Point ptEnd,DrawingContext dc, Color color)
        {
            Pen myPen = new Pen(new SolidColorBrush(color), 2.0F);
            //myPen.DashStyle = DashStyles.Dot;
            dc.DrawLine(myPen, ptStart, ptEnd);
        }
    }