using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WFbind
{
    public abstract class TwoWayBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel> where TViewModel : INotifyPropertyChanged
    {
        protected TwoWayBinding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
        }

        internal override void UpdateViewModel()
        {
            base.UpdateViewModel();
            var viewModel = BindingManager.GetViewModelFor<TViewModel>(View);

            var valueToSet = ViewPropertyInfo.GetValue(Control);
            ViewModelPropertyInfo.SetValue(viewModel, valueToSet);
        }
    }
}