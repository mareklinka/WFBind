using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFBind.Bindings
{
    /// <summary>
    /// Command binding for toolstrip buttons.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    /// <typeparam name="TViewModel">Type of viewmodel to bind to.</typeparam>
    internal class ToolStripButtonCommandBinding<TView, TViewModel> : CommandBinding<TView, ToolStripButton, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new isntance of the ToolStripButtonCommandBinding class.
        /// </summary>
        /// <param name="view">View to bind to.</param>
        /// <param name="control">Control to bind to.</param>
        /// <param name="viewModel">Viewmodel to bind to.</param>
        /// <param name="viewModelProperty">Viewmodel property to bind to.</param>
        public ToolStripButtonCommandBinding(TView view, ToolStripButton control,
            TViewModel viewModel,
            Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel, viewModelProperty)
        {
            ToggleControlState(GetCommand().CanExecute());
        }

        /// <summary>
        /// Hooks the events necessary for this binding.
        /// </summary>
        protected internal sealed override void HookEvents()
        {
            base.HookEvents();
            Control.Click += ControlOnClick;
        }

        /// <summary>
        /// Toggles the bound control's state according to the current state of the command.
        /// </summary>
        /// <param name="canExecute">Current state of the bound command.</param>
        protected sealed override void ToggleControlState(bool canExecute)
        {
            Control.Enabled = canExecute;
        }

        /// <summary>
        /// Handles the toolstrip button's Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ControlOnClick(object sender, EventArgs eventArgs)
        {
            var command = GetCommand();

            if (command.CanExecute())
            {
                command.Execute();
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

        /// <summary>
        /// Unhooks all previously hooked events.
        /// </summary>
        protected sealed override void UnhookEvents()
        {
            Control.Click -= ControlOnClick;
            base.UnhookEvents();
        }
    }
}