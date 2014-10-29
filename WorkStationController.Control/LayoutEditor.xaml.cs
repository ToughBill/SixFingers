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
using WorkstationController.VisualElement.Uitility;

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
            
            uiContainer.SizeChanged += uiContainer_SizeChanged;
            uiController = new UIMovementsController(uiContainer);
            uiController.onLabelPreviewChanged += uiController_onLabelPreviewChanged; ;
            this.Loaded += LayoutUserControl_Loaded;
        }

        void uiController_onLabelPreviewChanged(object sender, EventArgs e)
        {
            LabwareUIElement labwareUIElement = (e as LabelChangeEventArgs).LabwareUIElement;
            QueryNewLabelForm queryNewLabelForm = new QueryNewLabelForm(uiContainer.Children, labwareUIElement);
            queryNewLabelForm.ShowDialog();
        }

        public bool AllowPickup 
        {
            get
            {
                return uiController.AllowPickup;
            }
            set
            {
                uiController.AllowPickup = value;
            }
        }


        void LayoutUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //to do, replace it by load the worktable from a xml
            Worktable worktable = new Worktable(
                                         new Size(8000, 3500),
                                         new Size(5, 30),
                                         new Size(5, 50),
                                         new Size(5, 50), new Point(500, 500), 1500, 2500, 20);

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
        /// <param name="labwareTypeName"></param>
        public void SuggestCandidate(WareBase wareBase)
        {
            var uiElement = UIElementFactory.CreateUIElement(wareBase,uiContainer.Children);
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
            double y = Mouse.GetPosition(this).Y;
            dc.DrawText(new FormattedText(string.Format("x:{0} y:{1}",x,y),
                       CultureInfo.GetCultureInfo("en-us"),
                       FlowDirection.LeftToRight,
                       new Typeface("Verdana"),
                       15, System.Windows.Media.Brushes.DarkBlue),
                       new System.Windows.Point(ActualWidth-150, ActualHeight-100));

        }
    }
}
