using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFBind.Bindings
{
    internal class ToolStripButtonCommandBinding<TView, TViewModel> : CommandBinding<TView, ToolStripButton, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public ToolStripButtonCommandBinding(TView view, ToolStripButton control,
            TViewModel viewModel,
            Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel, viewModelProperty)
        {
            HookEvents(control);
            ToggleCommandState(GetCommand().CanExecute());
        }

        private void HookEvents(ToolStripButton control)
        {
            control.Click += ControlOnClick;

        }

        protected override void ToggleCommandState(bool canExecute)
        {
            Control.Enabled = canExecute;
        }

        private void ControlOnClick(object sender, EventArgs eventArgs)
        {
            var command = GetCommand();

            if (command.CanExecute())
            {
                command.Execute();
            }
        }

        internal override void Unbind()
        {
            UnhookEvents(Control);
            base.Unbind();
        }

        private void UnhookEvents(ToolStripButton control)
        {
            control.Click -= ControlOnClick;
            var command = GetCommand();
        }
    }
}