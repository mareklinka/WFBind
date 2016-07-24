using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFbind;
using WFBind.Exceptions;

namespace WFBind.Bindings
{
    /// <summary>
    /// Binding for textbox's Text property.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    /// <typeparam name="TViewModel">Type of viewmodel to bind to.</typeparam>
    internal sealed class TextBoxBinding<TView, TViewModel> : TwoWayBinding<TView, TextBox, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the TextBoxBinding class.
        /// </summary>
        /// <param name="view">View to bind to.</param>
        /// <param name="control">Control to bind to.</param>
        /// <param name="viewProperty"></param>
        /// <param name="viewModel">Viewmodel to bind to.</param>
        /// <param name="viewModelProperty">Viewmodel property to bind to.</param>
        public TextBoxBinding(TView view, TextBox control, Expression<Func<TextBox, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            Debug.Assert(ViewPropertyInfo.Name == "Text");
        }

        /// <summary>
        /// Hooks the events necessary for this binding.
        /// </summary>
        protected internal override void HookEvents()
        {
            Control.TextChanged += ControlOnTextChanged;
            Control.LostFocus += ControlOnLostFocus;
        }

        /// <summary>
        /// Handles the textbox's LostFocus event.
        /// </summary>
        private void ControlOnLostFocus(object sender, EventArgs eventArgs)
        {
            if (Configuration.UpdateSourceTrigger == UpdateSourceType.LostFocus)
            {
                UpdateViewModel();
            }
        }

        /// <summary>
        /// Updates the viewmodel with the curent value from the view.
        /// </summary>
        protected override void UpdateViewModel()
        {
            if (Configuration.IsTwoWay)
            {
                base.UpdateViewModel();
            }
        }

        protected override void UnhookEvents()
        {
            Control.TextChanged -= ControlOnTextChanged;
            Control.LostFocus -= ControlOnLostFocus;
        }

        /// <summary>
        /// Handles the textbox's TextChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ControlOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (Configuration.UpdateSourceTrigger == UpdateSourceType.OnPropertyChanged)
            {
                UpdateViewModel();
            }
        }

        /// <summary>
        /// Unbinds this binding.
        /// </summary>
        internal override void Unbind()
        {
            UnhookEvents();
            base.Unbind();
        }
    }
}