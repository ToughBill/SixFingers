using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.VisualElement;
using WorkstationController.VisualElement.ContextMenu;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for Layout.xaml
    /// </summary>
    public partial class RecipeEditor : UserControl , IDisposable
    {
        UIMovementsController           _uiController = null;
        WareContextMenuController       _contextMenuController = null;

        #region events
        public WareContextMenuController ContextMenuController 
        { 
            get
            {
                return _contextMenuController;
            }
        }
        #endregion
        /// <summary>
        /// Default constructor
        /// </summary>
        public RecipeEditor(Recipe recipe = null)
        {
            InitializeComponent();
            
            _worktable.SizeChanged += uiContainer_SizeChanged;
            _uiController = new UIMovementsController(_worktable, recipe);
            _contextMenuController = new WareContextMenuController(_uiController);
            //_uiController.onLabelPreviewChanged += uiController_onLabelPreviewChanged;
            this.Loaded += LayoutUserControl_Loaded;
        }

       
        /// <summary>
        /// whether we allow other form select our wares
        /// </summary>
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

        private Recipe GetLayoutPartOfRecipe()
        {
            Recipe layout = new Recipe();

            //get carriers
            foreach (UIElement uiElement in _worktable.Children)
            {
                if(!(uiElement is BasewareUIElement))
                    continue;
                if (uiElement is CarrierUIElement)
                {
                    var carrierUIElement = (CarrierUIElement)uiElement;
                    layout.AddCarrier(carrierUIElement.Carrier);
                }
            }
            return layout;
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
        
            _worktable.AttachWorktableVisual();
            _worktable.MouseMove += uiContainer_MouseMove;
            _worktable.UpdateLayout();
            OnContainerSizeChanged(new Size(500, 500)); //give default size
            _worktable.RenderSize = new Size(_worktable.RenderSize.Width + 1, _worktable.RenderSize.Height + 1); //force update
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

        public void Dispose()
        {
            _contextMenuController.Dispose();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Recipe recipe = (Recipe)GetLayoutPartOfRecipe();
            recipe.Name = txtRecipeName.Text;
            string xmlFilePath = string.Empty;
            try
            {
                if (InstrumentsManager.Instance.FindInstrument<LiquidClass>(recipe.ID, out xmlFilePath))
                {
                    recipe.Serialize(xmlFilePath);
                }
                else
                    InstrumentsManager.Instance.SaveInstrument(recipe);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
