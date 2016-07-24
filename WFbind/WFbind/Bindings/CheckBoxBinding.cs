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
    /// Binding for checkbox's Checked property.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    /// <typeparam name="TViewModel">Type of viewmodel to bind to.</typeparam>
    internal sealed class CheckBoxBinding<TView, TViewModel> : TwoWayBinding<TView, CheckBox, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the CheckBoxBinding class.
        /// </summary>
        /// <param name="view">View to bind to.</param>
        /// <param name="control">Control to bind to.</param>
        /// <param name="viewProperty"></param>
        /// <param name="viewModel">Viewmodel to bind to.</param>
        /// <param name="viewModelProperty">Viewmodel property to bind to.</param>
        public CheckBoxBinding(TView view, CheckBox control, Expression<Func<CheckBox, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            Debug.Assert(ViewPropertyInfo.Name == "Checked");
        }

        /// <summary>
        /// Hooks the events necessary for this binding.
        /// </summary>
        protected internal override void HookEvents()
        {
            base.HookEvents();
            Control.CheckedChanged += ControlOnCheckedChanged;
        }

        /// <summary>
        /// Handles the checkbox's Checked event.
        /// </summary>
        private void ControlOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            UpdateViewModel();
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

        /// <summary>
        /// Unhooks all previously hooked events.
        /// </summary>
        protected override void UnhookEvents()
        {
            Control.CheckedChanged -= ControlOnCheckedChanged;
            base.UnhookEvents();
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