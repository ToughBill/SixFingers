using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WorkstationController.Core.Data;

namespace WorkstationController.VisualElement
{
    public class BasewareUIElement : UIElement
    {
        /// <summary>
        /// Visual collection
        /// </summary>
        protected VisualCollection _children = null;

        /// <summary>
        /// Worktable data
        /// </summary>
        protected Worktable _worktable = null;
        protected bool _isSelected = false;
        private WareBase _ware = null;

        public BasewareUIElement(WareBase ware)
        {
            _ware = ware;
            _children = new VisualCollection(this);
            this._worktable = Configurations.Instance.Worktable;
            ware.PropertyChanged += ware_PropertyChanged;
            
        }

        void ware_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

            /// <summary>
        /// Gets or sets the label of the ware
        /// </summary>
        public string Label
        {
            get
            {
                return _ware.Label;
            }
            set
            {
                _ware.Label = value;
                InvalidateVisual();
            }
        }

        ///// <summary>
        ///// Rerender the carrier, template method.
        ///// </summary>
        //public void Update()
        //{
        //    if (_children.Count > 0)
        //        Render((DrawingVisual)_children[0]);
        //}

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_children.Count > 0)
                Render((DrawingVisual)_children[0]);
        }

        /// <summary>
        /// Create visual element,template method.
        /// </summary>
        /// <returns></returns>
        protected Visual CreateViusal()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Render(drawingVisual);
            return drawingVisual;
        }

        /// <summary>
        /// whether the UIElement is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                InvalidateVisual();
            }
        }


        /// <summary>
        /// derived class overwrite this to give different outlook.
        /// </summary>
        /// <param name="drawingVisual"></param>
        protected virtual void Render(DrawingVisual drawingVisual)
        {

        }


        #region visual container's MUST methods
        /// <summary>
        /// Gets the child visuals
        /// </summary>
        public VisualCollection Visuals
        {
            get
            {
                return _children;
            }
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
        #endregion
    }
}
