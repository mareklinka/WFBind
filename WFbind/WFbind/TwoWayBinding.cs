using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WFbind
{
    public abstract class TwoWayBinding<TView, TControl, TViewModel> : BindingBuilder<TView, TControl, TViewModel> where TViewModel : INotifyPropertyChanged
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