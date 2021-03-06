﻿using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WorkstationController.VisualElement
{

    public class BorderAdorner : Adorner
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="adornedElement"></param>
        public BorderAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender 
        // method, which is called by the layout system as part of a rendering pass. 
        protected override void OnRender(DrawingContext drawingContext)
        {
            UIElement uiElement = (UIElement)(this.AdornedElement);
            Rect adornedElementRect = GetRectOfObject(uiElement);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Red), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        private Rect GetRectOfObject(UIElement _element)
        {
            Rect rectangleBounds = new Rect();
            rectangleBounds = _element.RenderTransform.TransformBounds(new Rect(_element.RenderSize));
            return rectangleBounds;
        }
    }
}
