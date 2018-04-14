using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkstationController.Core.Data;
using WorkstationController.Core.Utility;
using WorkstationController.VisualElement;
using WorkstationController.VisualElement.Uitility;

namespace WorkstationController.Control
{
    /// <summary>
    /// Interaction logic for Layout.xaml
    /// </summary>
    public partial class LayoutEditor : BaseEditor
    {
        UIMovementsController     _uiController = null;
        //Recipe                    _recipe = null;
        Layout _layout = null;
        #region events
   

        /// <summary>
        /// Event of edit labware and carrier
        /// </summary>
        public event EventHandler<WareBase> EditWare = delegate { };
       
        #endregion
        /// <summary>
        /// Default constructor
        /// </summary>
        public LayoutEditor(Layout layout = null, NewInformationHandler newInfoHandler = null)
            : base(newInfoHandler)
        {
            InitializeComponent();
            _layout = layout;
            if (_layout == null)
                _layout = new Layout();
           
            _worktable.SizeChanged += uiContainer_SizeChanged;
            _uiController = new UIMovementsController(_worktable, _layout);
            _uiController.onLabelPreviewChanged += uiController_onLabelPreviewChanged;
            
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

        /// <summary>
        /// editing recipe
        /// </summary>
        public Layout Layout 
        { 
            get
            {
                return _layout;
            }
        }

        private List<Carrier> GetCarriers()
        {
            List<Carrier> carriers = new List<Carrier>();
            //get carriers
            foreach (UIElement uiElement in _worktable.Children)
            {
                if (uiElement is CarrierUIElement)
                {
                    var carrierUIElement = (CarrierUIElement)uiElement;
                    carriers.Add(carrierUIElement.Carrier);
                }
            }
            return carriers;
        }
       
        
        /// <summary>
        /// suggest candidate
        /// </summary>
        /// <param name="wareBase">A WareBase instance</param>
        public void SuggestCandidate(WareBase wareBase)
        {
            var uiElement = UIElementFactory.CreateUIElement(wareBase, _worktable.Children);
            int zIndex = uiElement is CarrierUIElement ? 10 : 20;
            Grid.SetZIndex(uiElement, zIndex);
            _uiController.UIElementCandidate = uiElement;
            Mouse.OverrideCursor = Cursors.Hand;
            _uiController.CaptureMouse();
        }

        #region form events
        private void LayoutUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //to do, replace it by load the worktable from a xml
            txtLayoutName.DataContext = _layout;
            _worktable.AttachWorktableVisual();
            _worktable.MouseMove += uiContainer_MouseMove;
            _worktable.UpdateLayout();
             OnContainerSizeChanged(new Size(500, 500)); //give default size
            _worktable.RenderSize = new Size(_worktable.RenderSize.Width + 1, _worktable.RenderSize.Height + 1); //force update
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_layout.SaveName == null || _layout.SaveName == "" || _layout.SaveName.Contains("<") || _layout.SaveName.Contains(">"))
            {
                string errMsg = "未为layout设置名称！";
                if(newInfoHandler != null)
                {
                    newInfoHandler(errMsg,true);
                }
                else
                {
                    throw new Exception(errMsg);
                }
            }
            List<Carrier> carriers = GetCarriers();
            _layout.Carriers = carriers;
            
            try
            {
                PipettorElementManager.Instance.SavePipettorElement(_layout);
            }
            catch(Exception ex)
            {
                 if (newInfoHandler != null)
                    newInfoHandler(ex.Message, true);
            }
        }

     

        #endregion

        #region sizeEvents
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
        #endregion



        #region control events
        private void uiController_onLabelPreviewChanged(object sender, EventArgs e)
        {
            LabwareUIElement labwareUIElement = (e as LabelChangeEventArgs).LabwareUIElement;
            QueryNewLabelForm queryNewLabelForm = new QueryNewLabelForm(_worktable.Children, labwareUIElement);
            queryNewLabelForm.ShowDialog();
        }

        #endregion

        public async void RunScript(List<IPipettorCommand> commands)
        {
            ScriptVisualizer scrpitVisualizer = new ScriptVisualizer(_worktable, commands);
            for (int i = 0; i < commands.Count; i++)
            {
                scrpitVisualizer.ExecuteOneCommand();
                await PutTaskDelay();
            }
            scrpitVisualizer.Finish();
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(1000);

        } 
    }

    #region worktable render
    /// <summary>
    /// render the worktable
    /// </summary>
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

            //double x = Mouse.GetPosition(this).X;
            //double y = Mouse.GetPosition(this).Y;
            //dc.DrawText(new FormattedText(string.Format("x:{0} y:{1}",x,y),
            //           CultureInfo.GetCultureInfo("en-us"),
            //           FlowDirection.LeftToRight,
            //           new Typeface("Verdana"),
            //           15, System.Windows.Media.Brushes.DarkBlue),
            //           new System.Windows.Point(ActualWidth-150, ActualHeight-100));
        }
    }
    #endregion
}
