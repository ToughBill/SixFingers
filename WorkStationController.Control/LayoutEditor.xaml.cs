using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkstationController.Core.Data;
using WorkstationController.VisualElement;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for Layout.xaml
    /// </summary>
    public partial class LayoutEditor : UserControl
    {
        UIMovementsController uiController;
        /// <summary>
        /// c'tor
        /// </summary>
        public LayoutEditor()
        {
            InitializeComponent();
            uiController = new UIMovementsController(uiContainer);
            uiContainer.SizeChanged += uiContainer_SizeChanged;
            this.Loaded += LayoutUserControl_Loaded;
        }


        void LayoutUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //to do, replace it by load the worktable from a xml
            Worktable worktable = new Worktable(
                                         new Size(8000, 3000),
                                         new Size(5, 30),
                                         new Size(5, 50),
                                         new Size(5, 50), new Point(500, 500), 1500, 2500, 28);

            Configurations.Instance.Worktable = worktable;
            uiContainer.AttachWorktableVisual();
            uiContainer.MouseMove += uiContainer_MouseMove;
            OnContainerSizeChanged(new Size(800, 600));
        }

        void uiContainer_MouseMove(object sender, MouseEventArgs e)
        {
            uiContainer.InvalidateVisual();
        }


        void uiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnContainerSizeChanged(e.NewSize);
        }

        private void OnContainerSizeChanged(Size newSize)
        {
            if (Configurations.Instance.Worktable == null)
                return;
            
            VisualCommon.UpdateContainerSize(newSize);
            uiContainer.InvalidateVisual();
            VisualCommon.UpdateVisuals(uiContainer.Children);
        }
        
        /// <summary>
        /// suggest candidate
        /// </summary>
        /// <param name="uiElement"></param>
        public void AddCandidate(BasewareUIElement uiElement)
        {
            Debug.WriteLine("Add candidate");
            //uiController.RemoveCurrentSelectFlag();
            uiController.UIElementCandidate = uiElement;
            Mouse.OverrideCursor = Cursors.Hand;
            uiController.CaptureMouse();
        }
       
    }

    public class WorktableGrid : Grid
    {
        WorktableVisual worktableVisual = null;
        public void AttachWorktableVisual()
        {
            this.worktableVisual = new WorktableVisual();
        }
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            if (worktableVisual != null)
                worktableVisual.Draw(dc);

            double x = Mouse.GetPosition(this).X;
            dc.DrawText(new FormattedText(x.ToString(),
                       CultureInfo.GetCultureInfo("en-us"),
                       FlowDirection.LeftToRight,
                       new Typeface("Verdana"),
                       20, System.Windows.Media.Brushes.DarkBlue),
                       new System.Windows.Point(ActualWidth-100, ActualHeight-100));

        }
    }
}
