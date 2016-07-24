using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace WFbind
{
    /// <summary>
    /// Binding builder specifying binding control.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    public class BindingBuilder<TView>
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        protected TView View { get; }

        /// <summary>
        /// Initializes a new instance of the BindingBuilder type.
        /// </summary>
        /// <param name="view">The view to bind to.</param>
        internal BindingBuilder(TView view)
        {
           View = view;
        }

        /// <summary>
        /// Configures the binding to bind to the specified control.
        /// </summary>
        /// <typeparam name="TControl">Type of control to bind to.</typeparam>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewProperty">The target property in the control to bind to.</param>
        /// <returns>A binding builder for specifying viewmodel.</returns>
        public BindingBuilder<TView, TControl> Bind<TControl>(TControl control, Expression<Func<TControl, object>> viewProperty)
        {
            return new BindingBuilder<TView, TControl>(View, control, viewProperty);
        }

        /// <summary>
        /// Configures the binding to be a command binding for the specified control.
        /// </summary>
        /// <typeparam name="TButton">The type of control to bind to.</typeparam>
        /// <param name="item">The control to bind to.</param>
        /// <returns>A new command binding builder.</returns>
        public CommandBindingBuilder<TView, TButton> BindCommand<TButton>(TButton item)
        {
            return new CommandBindingBuilder<TView, TButton>(View, item);
        }
    }

    /// <summary>
    /// Binding builder specifying binding viewmodel.
    /// </summary>
    /// <typeparam name="TView">Type of view to bind to.</typeparam>
    /// <typeparam name="TControl">Type of control to bind to.</typeparam>
    public class BindingBuilder<TView, TControl> : BindingBuilder<TView>
    {
        /// <summary>
        /// Gets the control.
        /// </summary>
        protected TControl Control { get; }

        /// <summary>
        /// Gets the expression specifying the control property to bind to.
        /// </summary>
        protected Expression<Func<TControl, object>> ViewProperty { get; }

        /// <summary>
        /// Creates a new instance of the BindingBuilder class.
        /// </summary>
        /// <param name="view">The view to bind to.</param>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewProperty">The control's property to bind to.</param>
        /// <exception cref="ArgumentNullException">Thrown when the control or viewProperty is null.</exception>
        internal BindingBuilder(TView view, TControl control, Expression<Func<TControl, object>> viewProperty) : base(view)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            Control = control;
            ViewProperty = viewProperty;
        }

        /// <summary>
        /// Configures the binding to bind to the specified viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel">Type of the viewmodel to bind to.</typeparam>
        /// <param name="viewModel">The viewmodel to bind to.</param>
        /// <param name="viewModelProperty">The viewmodel' property to bind to.</param>
        /// <exception cref="ArgumentNullException">Thrown when the viewModel or viewModelProperty is null.</exception>
        /// <returns>The constructed binding.</returns>
        public Binding To<TViewModel>(TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty == null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            var binding = BindingFactory.Build(View, Control, ViewProperty, viewModel, viewModelProperty);

            BindingManager.AddBinding(binding);

            return binding;
        }
    }
}