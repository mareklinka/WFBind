using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFBind.Bindings
{
    internal class ButtonCommandBinding<TView, TViewModel> : CommandBinding<TView, Button, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public ButtonCommandBinding(TView view, Button control,
            TViewModel viewModel,
            Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel, viewModelProperty)
        {
            HookEvents(control);
            ToggleCommandState(GetCommand().CanExecute());
        }

        private void HookEvents(Button control)
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

        private void UnhookEvents(Button control)
        {
            control.Click -= ControlOnClick;
            var command = GetCommand();
        }
    }
}