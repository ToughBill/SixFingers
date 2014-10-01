using System;
using System.Collections.Generic;
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
        WorktableVisual wktblVisual;
       
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
                                         new Size(4000, 8000),
                                         new Size(20, 40),
                                         new Size(20, 60),
                                         new Size(20, 60), new Point(50, 50), 1500, 3500, 20);

            Configurations.Instance.Worktable = worktable;
            wktblVisual = new WorktableVisual();
            OnContainerSizeChanged(new Size(800, 600));
        }

        void uiContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnContainerSizeChanged(e.NewSize);
        }

        private void OnContainerSizeChanged(Size newSize)
        {
            if (wktblVisual == null)
                return;
            VisualCommon.UpdateContainerSize(newSize);
            VisualCommon.UpdateVisuals(uiContainer.Children);
        }
        /// <summary>
        /// for test
        /// </summary>
        /// <param name="uiElement"></param>
        public void AddNewElement(UIElement uiElement)
        {
            uiController.NewUIElement = uiElement;
            
        }

        /// <summary>
        /// overwrite OnRender to update our uiElements automatically
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (wktblVisual != null)
                wktblVisual.Draw(drawingContext);
            VisualCommon.UpdateVisuals(uiContainer.Children);
        }
    }



}
