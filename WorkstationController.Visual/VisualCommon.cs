﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
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
                    basewareUIElement.Update();
            }
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
        public static void DrawRect(int x, int y, Size size, DrawingContext drawingContext, Color color, Brush brush = null)
        {
            double xPixel = VisualCommon.Convert2PixelXUnit(x);
            double yPixel = VisualCommon.Convert2PixelYUnit(y);
            xPixel += GetXShift();
            yPixel += GetYShift();
            double wPixel = VisualCommon.Convert2PixelXUnit(size.Width);
            double hPixel = VisualCommon.Convert2PixelYUnit(size.Height);
            if (brush == null)
                brush = Brushes.Transparent;
            drawingContext.DrawRectangle(brush, new Pen(new SolidColorBrush(color), 1),
                new Rect(new Point(xPixel, yPixel), new Size(wPixel, hPixel)));
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
            drawingContext.DrawEllipse(brush, new Pen(new SolidColorBrush(color), 1), new Point(xPixel, yPixel), rXPixel, rYPixel);
        }

        private static double GetXShift()
        {
            return 0.1 * containerSize.Width;
        }

        private static double GetYShift()
        {
            return 0.1 * containerSize.Height;
        }
    }
}
