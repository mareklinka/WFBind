using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using WFbind;

namespace WFBind.Bindings
{
    public abstract class CommandBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel> where TViewModel : INotifyPropertyChanged
    {
        protected PropertyInfo ViewModelCommandPropertyInfo { get; private set; }
        protected Expression<Func<TViewModel, ICommand>> ViewModelCommandProperty { get; private set; }

        internal sealed override void UpdateView()
        {
        }

        internal sealed override void UpdateViewModel()
        {
        }

        internal sealed override bool IsAffectedBy(INotifyPropertyChanged viewModel, string propertyName)
        {
            // commands do not react to viewmodel updates
            return false;
        }

        protected CommandBinding(TView view, TControl control,
            TViewModel viewModel, Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel)
        {
            ViewModelCommandProperty = viewModelProperty;
            ViewModelCommandPropertyInfo = viewModel.GetPropertyInfo(viewModelProperty);

            HookEvents();
        }

        private void HookEvents()
        {
            var command = GetCommand();
            command.CanExecuteChanged += CommandOnCanExecuteChanged;
        }

        private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ToggleCommandState(GetCommand().CanExecute());
        }

        protected abstract void ToggleCommandState(bool canExecute);

        protected ICommand GetCommand()
        {
            return (ICommand)ViewModelCommandPropertyInfo.GetValue(ViewModel);
        }

        internal override void Unbind()
        {
            UnhookEvents();
            base.Unbind();
        }

        private void UnhookEvents()
        {
            var command = GetCommand();
            command.CanExecuteChanged -= CommandOnCanExecuteChanged;
        }
    }
}