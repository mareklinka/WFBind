using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFBind;
using WFBind.Bindings;

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

            if (typeof(TControl) == typeof(CheckBox) && control.GetPropertyInfo(viewProperty).Name == "Checked")
            {
                return new CheckBoxBinding<TView, TViewModel>(view, control as CheckBox, 
                    viewProperty as Expression<Func<CheckBox, object>>, viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(RadioButton) && control.GetPropertyInfo(viewProperty).Name == "Checked")
            {
                return new RadioBinding<TView, TViewModel>(view, control as RadioButton,
                    viewProperty as Expression<Func<RadioButton, object>>, viewModel, viewModelProperty);
            }

            return new SimpleBinding<TView, TControl, TViewModel>(view, control,
                viewProperty, viewModel, viewModelProperty);
        }

        public static Binding<TView> BuildCommand<TView, TControl, TViewModel>(TView view,
            TControl control,  TViewModel viewModel, Expression<Func<TViewModel, ICommand>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (typeof(TControl) == typeof(Button))
            {
                return new ButtonCommandBinding<TView, TViewModel>(view, control as Button,
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(ToolStripMenuItem))
            {
                return new ToolStripMenuItemCommandBinding<TView, TViewModel>(view, control as ToolStripMenuItem, 
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(ToolStripButton))
            {
                return new ToolStripButtonCommandBinding<TView, TViewModel>(view, control as ToolStripButton,
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(MenuItem))
            {
                return new MenuItemCommandBinding<TView, TViewModel>(view, control as MenuItem,
                    viewModel, viewModelProperty);
            }

            throw new NotSupportedException(string.Format("Command binding is not supported for {0}", typeof(TControl)));
        }
    }
}