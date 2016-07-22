using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFbind
{
    internal class LabelBinding<TView, TViewModel> : Binding<TView, Label, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public LabelBinding(TView view, Label control, Expression<Func<Label, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
        }
    }
}