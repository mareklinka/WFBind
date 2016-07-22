using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WFbind
{
    public class BindingBuilder<TView> : AbstractBindingBuilder
    {
        internal TView View { get; }

        internal BindingBuilder(TView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            View = view;
        }

        public BindingBuilder<TView, TControl> Bind<TControl>(TControl control, Expression<Func<TControl, object>> viewProperty)
        {
            return new BindingBuilder<TView, TControl>(View, control, viewProperty);
        }

        internal override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return false;
        }

        internal override void Unbind()
        {
        }

        internal override void Update()
        {
        }

        internal override bool IsAffectedBy(string propertyName)
        {
            return false;
        }
    }

    public class BindingBuilder<TView, TControl> : BindingBuilder<TView>
    {
        public TControl Control { get; }
        public Expression<Func<TControl, object>> ViewProperty { get; }

        internal BindingBuilder(TView view, TControl control, Expression<Func<TControl, object>> viewProperty) : base(view)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            Control = control;
            ViewProperty = viewProperty;
        }

        public FinalBinding To<TViewModel>(TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            var binding = Build(View, Control, ViewProperty, viewModel, viewModelProperty);

            BindingManager.AddBinding(binding);

            return binding;
        }
    }

    public class BindingBuilder<TView, TControl, TViewModel> : BindingBuilder<TView, TControl> where TViewModel : INotifyPropertyChanged
    {
        public Expression<Func<TViewModel, object>> ViewModelProperty { get; }

        public TViewModel ViewModel { get; }

        internal BindingBuilder(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty) : base(view, control, viewProperty)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty == null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            ViewModel = viewModel;
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
            return ViewModel.Equals(viewModel);
        }
    }
}