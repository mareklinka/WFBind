using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFBind;
using WFBind.Bindings;

namespace WFbind
{
    /// <summary>
    /// Binding factory.
    /// </summary>
    public static class BindingFactory
    {
        /// <summary>
        /// Builds a new binding.
        /// </summary>
        /// <typeparam name="TView">The type of the view the new binding binds to.</typeparam>
        /// <typeparam name="TControl">The type of the control the new binding binds to.</typeparam>
        /// <typeparam name="TViewModel">The type of the viewmodel the new binding binds to.</typeparam>
        /// <param name="view">The view to bind to.</param>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewProperty">The binding target property (the property on the control to bind to).</param>
        /// <param name="viewModel">The viewmodel to bind to.</param>
        /// <param name="viewModelProperty">The binding source property (the property on the viewmodel to bind to).</param>
        /// <returns>A new binding.</returns>
        internal static Binding<TView> Build<TView, TControl, TViewModel>(TView view,
            TControl control, Expression<Func<TControl, object>> viewProperty, TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            Binding<TView> result = null;

            // special cases are handled by type and property name
            if (typeof(TControl) == typeof(TextBox) && control.GetPropertyInfo(viewProperty).Name == "Text")
            {
                result = new TextBoxBinding<TView, TViewModel>(view, control as TextBox,
                    viewProperty as Expression<Func<TextBox, object>>, viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(CheckBox) && control.GetPropertyInfo(viewProperty).Name == "Checked")
            {
                result = new CheckBoxBinding<TView, TViewModel>(view, control as CheckBox, 
                    viewProperty as Expression<Func<CheckBox, object>>, viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(RadioButton) && control.GetPropertyInfo(viewProperty).Name == "Checked")
            {
                result = new RadioBinding<TView, TViewModel>(view, control as RadioButton,
                    viewProperty as Expression<Func<RadioButton, object>>, viewModel, viewModelProperty);
            }

            if (result != null)
            {
                result.HookEvents();
                return result;
            }

            // generic one-way binding
            return new SimpleBinding<TView, TControl, TViewModel>(view, control,
                viewProperty, viewModel, viewModelProperty);
        }

        /// <summary>
        /// Builds a new command binding.
        /// </summary>
        /// <typeparam name="TView">The type of the view the new binding binds to.</typeparam>
        /// <typeparam name="TControl">The type of the control the new binding binds to.</typeparam>
        /// <typeparam name="TViewModel">The type of the viewmodel the new binding binds to.</typeparam>
        /// <param name="view">The view to bind to.</param>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewModel">The viewmodel to bind to.</param>
        /// <param name="viewModelProperty">The binding source property (the property on the viewmodel to bind to).</param>
        /// <exception cref="NotSupportedException">Thrown when an unsupported control is encountered.</exception>
        /// <returns>A new command binding.</returns>
        internal static Binding<TView> BuildCommand<TView, TControl, TViewModel>(TView view,
            TControl control,  TViewModel viewModel, Expression<Func<TViewModel, ICommand>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            Binding<TView> result = null;

            // type of command binding is derived from the type of control
            if (typeof(TControl) == typeof(Button))
            {
                result = new ButtonCommandBinding<TView, TViewModel>(view, control as Button,
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(ToolStripMenuItem))
            {
                result = new ToolStripMenuItemCommandBinding<TView, TViewModel>(view, control as ToolStripMenuItem, 
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(ToolStripButton))
            {
                result = new ToolStripButtonCommandBinding<TView, TViewModel>(view, control as ToolStripButton,
                    viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(MenuItem))
            {
                result = new MenuItemCommandBinding<TView, TViewModel>(view, control as MenuItem,
                    viewModel, viewModelProperty);
            }

            if (result != null)
            {
                result.HookEvents();
                return result;
            }

            // unsupported control type
            throw new NotSupportedException(string.Format("Command binding is not supported for {0}", typeof(TControl)));
        }
    }
}