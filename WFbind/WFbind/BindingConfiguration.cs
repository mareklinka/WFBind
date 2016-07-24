namespace WFbind
{
    /// <summary>
    /// Binding configuration.
    /// </summary>
    public sealed class BindingConfiguration
    {
        /// <summary>
        /// Gets or sets the value indicating whether this binding should be a two-way binding. Default: false
        /// </summary>
        public bool IsTwoWay { get; set; } = false;

        /// <summary>
        /// Gets or sets the update source trigger  to use with the binding. Default: UpdateSourceType.OnPropertyChanged
        /// </summary>
        public UpdateSourceType UpdateSourceTrigger { get; set; } = UpdateSourceType.OnPropertyChanged;
    }

    /// <summary>
    /// The update source trigger type.
    /// </summary>
    public enum UpdateSourceType
    {
        /// <summary>
        /// The binding source is updated every time the current value changes in the view.
        /// </summary>
        OnPropertyChanged,

        /// <summary>
        /// The binding source is updated when the target control loses focus.
        /// </summary>
        LostFocus
    }
}