using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.VisualElement;


namespace WorkstationController.WorktableVisualTest
{
    public class WorktableGrid : Grid
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            double x = Mouse.GetPosition(this).X;
            double y = Mouse.GetPosition(this).Y;
            dc.DrawText(new FormattedText(string.Format("x:{0} y:{1}", x, y),
                       CultureInfo.GetCultureInfo("en-us"),
                       FlowDirection.LeftToRight,
                       new Typeface("Verdana"),
                       15, System.Windows.Media.Brushes.DarkBlue),
                       new System.Windows.Point(ActualWidth - 150, ActualHeight - 100));

        }
    }
}
