using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WFbind
{
    public class Binding<TView> : AbstractBinding
    {
        internal TView View { get; }

        public Binding(TView view)
        {
            View = view;
        }

        public Binding<TView, TControl> Bind<TControl>(TControl control, Expression<Func<TControl, object>> viewProperty)
        {
            return new Binding<TView, TControl>(View, control, viewProperty);
        }

        internal override bool IsAffectedBy(string propertyName)
        {
            return false;
        }

        internal override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return false;
        }

        internal override void Update()
        {
        }

        internal override void Unbind()
        {
        }
    }

    public class Binding<TView, TControl> : Binding<TView>
    {
        public TControl Control { get; }
        public Expression<Func<TControl, object>> ViewProperty { get; }

        public Binding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty) : base(view)
        {
            Control = control;
            ViewProperty = viewProperty;
        }

        public AbstractBinding To<TViewModel>(TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            var binding = Build(View, Control, ViewProperty, viewModel, viewModelProperty);

            BindingManager.AddBinding(binding);

            return binding;
        }

        internal override void Unbind()
        {
            base.Unbind();
        }
    }

    public class Binding<TView, TControl, TViewModel> : Binding<TView, TControl> where TViewModel : INotifyPropertyChanged
    {
        public Expression<Func<TViewModel, object>> ViewModelProperty { get; }

        public TViewModel VM { get; }

        public Binding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel vieWModel,
            Expression<Func<TViewModel, object>> viewModelProperty) : base(view, control, viewProperty)
        {
            VM = vieWModel;
            ViewModelProperty = viewModelProperty;
        }
        
        internal sealed override bool IsAffectedBy(string bindingPropertyName)
        {
            var viewModel = BindingManager.GetViewModelFor<TViewModel>(View);

            var viewModelProperty = viewModel.GetPropertyInfo(ViewModelProperty);

            return viewModelProperty.Name == bindingPropertyName;
        }

        internal sealed override void Update()
        {
            var viewModel = BindingManager.GetViewModelFor<TViewModel>(View);

            var viewModelProperty = viewModel.GetPropertyInfo(ViewModelProperty);
            var viewProperty = Control.GetPropertyInfo(ViewProperty);

            var valueToSet = viewModelProperty.GetValue(viewModel);
            viewProperty.SetValue(Control, valueToSet);
        }

        internal sealed override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return VM.Equals(viewModel);
        }
    }

    public abstract class TwoWayBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel> where TViewModel : INotifyPropertyChanged

    {
        protected TwoWayBinding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
        }

        protected override void UpdateSource()
        {
            base.UpdateSource();
            var viewModel = BindingManager.GetViewModelFor<TViewModel>(View);

            var viewModelProperty = viewModel.GetPropertyInfo(ViewModelProperty);
            var viewProperty = Control.GetPropertyInfo(ViewProperty);

            var valueToSet = viewProperty.GetValue(Control);
            viewModelProperty.SetValue(viewModel, valueToSet);
        }
    }
}