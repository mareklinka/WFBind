using System;
using System.ComponentModel;
using System.Linq.Expressions;
using WFbind;

namespace WFBind.Bindings
{
    /// <summary>
    /// Simple one-way binding.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    /// <typeparam name="TControl">Type of control to bind to.</typeparam>
    /// <typeparam name="TViewModel">Type of viewmodel to bind to.</typeparam>
    internal sealed class SimpleBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the SimpleBinding class.
        /// </summary>
        /// <param name="view">View to bind to.</param>
        /// <param name="control">Control to bind to.</param>
        /// <param name="viewProperty"></param>
        /// <param name="viewModel">Viewmodel to bind to.</param>
        /// <param name="viewModelProperty">Viewmodel property to bind to.</param>
        public SimpleBinding(TView view, TControl control, Expression<Func<TControl, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
        }
    }
}