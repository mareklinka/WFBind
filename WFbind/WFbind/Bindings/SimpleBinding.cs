using System;
using System.ComponentModel;
using System.Linq.Expressions;
using WFbind;

namespace WFBind.Bindings
{
    internal class SimpleBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public SimpleBinding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
        }
    }
}