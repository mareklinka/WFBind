using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFbind
{
    public abstract class AbstractBindingBuilder
    {
        public static Binding<TView> Build<TView, TControl, TViewModel>(TView view,
            TControl control, Expression<Func<TControl, object>> viewProperty, TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (typeof(TControl) == typeof(TextBox) && control.GetPropertyInfo(viewProperty).Name == "Text")
            {
                return new TextBoxBinding<TView, TViewModel>(view, control as TextBox,
                    viewProperty as Expression<Func<TextBox, object>>, viewModel, viewModelProperty);
            }

            return new SimpleBinding<TView, TControl, TViewModel>(view, control,
                viewProperty, viewModel, viewModelProperty);
        }
    }
}