using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFBind.Bindings
{
    /// <summary>
    /// Command binding for toolstip menu items.
    /// </summary>
    /// <typeparam name="TView">Tpye of view to bind to.</typeparam>
    /// <typeparam name="TViewModel">Type of viewmodel to bind to.</typeparam>
    internal sealed class ToolStripMenuItemCommandBinding<TView, TViewModel> : CommandBinding<TView, ToolStripMenuItem, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new isntance of the ToolStripMenuItemCommandBinding class.
        /// </summary>
        /// <param name="view">View to bind to.</param>
        /// <param name="control">Control to bind to.</param>
        /// <param name="viewModel">Viewmodel to bind to.</param>
        /// <param name="viewModelProperty">Viewmodel property to bind to.</param>
        public ToolStripMenuItemCommandBinding(TView view, ToolStripMenuItem control,
            TViewModel viewModel,
            Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel, viewModelProperty)
        {
            ToggleControlState(GetCommand().CanExecute());
        }

        protected internal override void HookEvents()
        {
            base.HookEvents();
            Control.Click += ControlOnClick;
        }

        /// <summary>
        /// Toggles the bound control's state according to the current state of the command.
        /// </summary>
        /// <param name="canExecute">Current state of the bound command.</param>
        protected override void ToggleControlState(bool canExecute)
        {
            Control.Enabled = canExecute;
        }

        /// <summary>
        /// Handles the toolstrip menu's Click event.
        /// </summary>
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
        protected override void UnhookEvents()
        {
            Control.Click -= ControlOnClick;
            base.UnhookEvents();
        }
    }
}