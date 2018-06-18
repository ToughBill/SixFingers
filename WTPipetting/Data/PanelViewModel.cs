using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WTPipetting.Data
{
    public class PanelViewModel : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        PanelViewModel _parent;
        bool bFreeezeEvent = false;
        #endregion // Data

        #region CreateFoos
        internal static PanelViewModel CreateViewModel(List<string> assays)
        {
            PanelViewModel root = new PanelViewModel("所有试验");
            foreach (var assay in assays)
            {
                PanelViewModel firstLevel = new PanelViewModel(assay);
                root.Children.Add(firstLevel);
            }
            root.Initialize();

            // Default all the assays are checked
            root.IsChecked = true;

            return root;
        }

        PanelViewModel(string name)
        {
            this.Name = name;





            this.Children = new List<PanelViewModel>();
        }

        void Initialize()
        {
            foreach (PanelViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #endregion // CreateFoos

        #region Properties

        public List<PanelViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            if (!bFreeezeEvent)
                this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        internal ObservableCollection<string> GetAssays()
        {
            ObservableCollection<string> assays = new ObservableCollection<string>();
            foreach (var firstLevel in this.Children)
            {
                if ((bool)firstLevel.IsChecked)
                {
                    assays.Add(firstLevel.Name);
                }
            }
            return assays;
        }
    }
}
