using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using WFBind;

namespace WFbind
{
    public abstract class Binding
    {
        public Binding Setup(Action<BindingConfiguration> setting)
        {
            setting(Configuration);
            return this;
        }

        internal abstract bool HasViewModel(INotifyPropertyChanged viewModel);

        internal abstract void Unbind();

        internal abstract void UpdateView();

        internal virtual void UpdateViewModel()
        {
        }

        internal abstract bool IsAffectedBy(INotifyPropertyChanged viewModel, string propertyName);

        internal abstract BindingConfiguration Configuration { get; }

        public abstract bool HasView(object view);
    }

    public abstract class Binding<TView> : Binding
    {
        internal abstract TView View { get; }
    }

    public abstract class Binding<TView, TControl, TViewModel> : Binding<TView> where TViewModel : INotifyPropertyChanged
    {
        internal TViewModel ViewModel { get; private set; }
        protected Expression<Func<TControl, object>> ViewProperty { get; private set; }
        protected Expression<Func<TViewModel, object>> ViewModelProperty { get; private set; }
        protected TControl Control { get; private set; }
        protected PropertyInfo ViewPropertyInfo { get; private set; }
        protected PropertyInfo ViewModelPropertyInfo { get; private set; }
        private TView _view;

        protected Binding(TView view, TControl control, TViewModel viewModel)
        {
            _view = view;
            Control = control;
            ViewModel = viewModel;
            Configuration = new BindingConfiguration();
        }

        protected Binding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty, TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) : this(view, control, viewModel)
        {
            ViewModel = viewModel;
            ViewProperty = viewProperty;
            Control = control;
            ViewPropertyInfo = Control.GetPropertyInfo(ViewProperty);
            ViewModelProperty = viewModelProperty;
            ViewModelPropertyInfo = ViewModel.GetPropertyInfo(ViewModelProperty);
        }

        internal override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return ViewModel.Equals(viewModel);
        }

        internal override bool IsAffectedBy(INotifyPropertyChanged vieWModel, string propertyName)
        {
            return ViewModel.Equals(vieWModel) && ViewModelPropertyInfo.Name == propertyName;
        }

        internal override BindingConfiguration Configuration { get; }

        public override bool HasView(object view)
        {
            return View.Equals(view);
        }

        internal override void UpdateView()
        {
            var valueToSet = ViewModelPropertyInfo.GetValue(ViewModel);
            ViewPropertyInfo.SetValue(Control, valueToSet);
        }

        internal override void Unbind()
        {
            _view = default(TView);
            ViewModel = default(TViewModel);
            ViewProperty = null;
            Control = default(TControl);
            ViewPropertyInfo = null;
            ViewModelProperty = null;
            ViewModelPropertyInfo = null;
        }

        internal override TView View
        {
            get
            {
                return _view;
            }
        }
    }
}