using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WorkstationController.Control.ThirdParties;
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
        UIMovementsController            _uiController = null;
        ListViewDragDropManager<Command> _dragMgr      = null;

        // Temp _worktable to make code compile
        //WorktableGrid _worktable = new WorktableGrid();
        
        // Temp commands for listview binding
        ObservableCollection<Command> _script = new ObservableCollection<Command>(
            new Command[] { new Command("Aspiration", "10, 3, 15, 20"), 
                            new Command("Dispense", "10, 3, 15, 20")});

        /// <summary>
        /// Default constructor
        /// </summary>
        public LayoutEditor()
        {
            InitializeComponent();
            
            _worktable.SizeChanged += uiContainer_SizeChanged;
            _uiController = new UIMovementsController(_worktable);
            _uiController.onLabelPreviewChanged += uiController_onLabelPreviewChanged;
            this.Loaded += LayoutUserControl_Loaded;
        }

        public bool AllowPickup 
        {
            get
            {
                return _uiController.AllowPickup;
            }
            set
            {
                _uiController.AllowPickup = value;
            }
        }

      
        
        /// <summary>
        /// suggest candidate
        /// </summary>
        /// <param name="wareBase">A WareBase instance</param>
        public void SuggestCandidate(WareBase wareBase)
        {
            var uiElement = UIElementFactory.CreateUIElement(wareBase, _worktable.Children);
            _uiController.UIElementCandidate = uiElement;
            Mouse.OverrideCursor = Cursors.Hand;
            _uiController.CaptureMouse();
        }

        private void uiController_onLabelPreviewChanged(object sender, EventArgs e)
        {
            LabwareUIElement labwareUIElement = (e as LabelChangeEventArgs).LabwareUIElement;
            QueryNewLabelForm queryNewLabelForm = new QueryNewLabelForm(_worktable.Children, labwareUIElement);
            queryNewLabelForm.ShowDialog();
        }

        private void LayoutUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //to do, replace it by load the worktable from a xml
            Worktable worktable = new Worktable(
                                         new Size(8000, 3500),
                                         new Size(5, 30),
                                         new Size(5, 50),
                                         new Size(5, 50), new Point(500, 500), 1500, 2500, 20);

            Configurations.Instance.Worktable = worktable;
            _worktable.AttachWorktableVisual();
            _worktable.MouseMove += uiContainer_MouseMove;
            OnContainerSizeChanged(new Size(800, 600));
        }

        private void uiContainer_MouseMove(object sender, MouseEventArgs e)
        {
            _worktable.InvalidateVisual();
        }

        private void uiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnContainerSizeChanged(e.NewSize);
        }

        private void OnContainerSizeChanged(Size newSize)
        {
            if (Configurations.Instance.Worktable == null)
                return;

            VisualCommon.UpdateContainerSize(newSize);
            _worktable.InvalidateVisual();
            VisualCommon.UpdateVisuals(_worktable.Children);
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
