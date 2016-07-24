using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using WFbind;

namespace WFBind.Bindings
{
    internal abstract class CommandBinding<TView, TControl, TViewModel> : Binding<TView, TControl, TViewModel> where TViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the property info for the view's command property.
        /// </summary>
        protected PropertyInfo ViewModelCommandPropertyInfo { get; private set; }

        /// <summary>
        /// Gets the view's command property.
        /// </summary>
        protected Expression<Func<TViewModel, ICommand>> ViewModelCommandProperty { get; private set; }

        protected CommandBinding(TView view, TControl control,
            TViewModel viewModel, Expression<Func<TViewModel, ICommand>> viewModelProperty)
            : base(view, control, viewModel)
        {
            ViewModelCommandProperty = viewModelProperty;
            ViewModelCommandPropertyInfo = viewModel.GetPropertyInfo(viewModelProperty);
        }

        /// <summary>
        /// Propagates a new value from viewmodel to view.
        /// </summary>
        internal sealed override void UpdateView()
        {
        }

        /// <summary>
        /// Checks whether this vinding is affected by a change of the specified property in the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to check.</param>
        /// <param name="propertyName">The property to checks.</param>
        /// <returns>True if this binding binds to the specified viewmodel and the specified property, otherwise false.</returns>
        internal sealed override bool IsAffectedBy(INotifyPropertyChanged viewModel, string propertyName)
        {
            // commands do not react to viewmodel updates
            return false;
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
        /// Propagates a new value from view to viewmodel.
        /// </summary>
        protected sealed override void UpdateViewModel()
        {
        }

        /// <summary>
        /// Toggles the bound control's state according to the current state of the command.
        /// </summary>
        /// <param name="canExecute">Current state of the bound command.</param>
        protected abstract void ToggleControlState(bool canExecute);

        /// <summary>
        /// Gets the boudn command.
        /// </summary>
        /// <returns>The bound command.</returns>
        protected ICommand GetCommand()
        {
            return (ICommand)ViewModelCommandPropertyInfo.GetValue(ViewModel);
        }
        
        /// <summary>
        /// Unhooks all previously hooked events.
        /// </summary>
        protected override void UnhookEvents()
        {
            var command = GetCommand();
            command.CanExecuteChanged -= CommandOnCanExecuteChanged;
            base.UnhookEvents();
        }

        /// <summary>
        /// Hooks the events necessary for this binding.
        /// </summary>
        protected internal override void HookEvents()
        {
            base.HookEvents();
            var command = GetCommand();
            command.CanExecuteChanged += CommandOnCanExecuteChanged;
        }

        /// <summary>
        /// Handles the command's CanExecuteChanged event.
        /// </summary>
        private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ToggleControlState(GetCommand().CanExecute());
        }
    }
}