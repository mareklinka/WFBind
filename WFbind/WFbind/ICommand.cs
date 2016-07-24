using System;

namespace WFBind
{
    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        event EventHandler CanExecuteChanged;

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        bool CanExecute();

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        void Execute();

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
