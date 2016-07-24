using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace WFbind
{
    /// <summary>
    /// Base class for all bindings.
    /// </summary>
    public abstract class Binding
    {
        /// <summary>
        /// Configures the binding.
        /// </summary>
        /// <param name="setting">The configuration to perform on the binding.</param>
        /// <returns>Self.</returns>
        public Binding Setup(Action<BindingConfiguration> setting)
        {
            setting(Configuration);
            return this;
        }

        /// <summary>
        /// Checks whether this binding binds to the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to check.</param>
        /// <returns>True if this binding binds to the specified viewmodel, otherwise false.</returns>
        internal abstract bool HasViewModel(INotifyPropertyChanged viewModel);

        /// <summary>
        /// Unbinds this binding.
        /// </summary>
        internal abstract void Unbind();

        /// <summary>
        /// Propagates a new value from viewmodel to view.
        /// </summary>
        internal abstract void UpdateView();

        /// <summary>
        /// Propagates a new value from view to viewmodel.
        /// </summary>
        protected virtual void UpdateViewModel()
        {
        }

        /// <summary>
        /// Checks whether this vinding is affected by a change of the specified property in the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to check.</param>
        /// <param name="propertyName">The property to checks.</param>
        /// <returns>True if this binding binds to the specified viewmodel and the specified property, otherwise false.</returns>
        internal abstract bool IsAffectedBy(INotifyPropertyChanged viewModel, string propertyName);

        /// <summary>
        /// Gets the binding configuratiion.
        /// </summary>
        protected abstract BindingConfiguration Configuration { get; }

        /// <summary>
        /// checks whether this binding binds to the specified view.
        /// </summary>
        /// <param name="view">The view to check.</param>
        /// <returns>Ture if this binding binds to the specified view, otherwise false.</returns>
        internal abstract bool HasView(object view);

        /// <summary>
        /// Unhooks all previously hooked events.
        /// </summary>
        protected virtual void UnhookEvents()
        {
        }

        /// <summary>
        /// Hooks the events necessary for this binding.
        /// </summary>
        protected internal virtual void HookEvents()
        {
        }
    }

    /// <summary>
    /// Binding with a view.
    /// </summary>
    /// <typeparam name="TView">The type of view this binging binds to.</typeparam>
    internal abstract class Binding<TView> : Binding
    {
        /// <summary>
        /// Gets the view this binding binds to.
        /// </summary>
        internal abstract TView View { get; }
    }

    /// <summary>
    /// Binding with view, control, and viewmodel.
    /// </summary>
    /// <typeparam name="TView">The type of view this binging binds to.</typeparam>
    /// <typeparam name="TControl">The type of control this binding binds to.</typeparam>
    /// <typeparam name="TViewModel">The type of viewmodel this binding binds to.</typeparam>
    internal abstract class Binding<TView, TControl, TViewModel> : Binding<TView> where TViewModel : INotifyPropertyChanged
    {
        private TView _view;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="view">The view to bind to.</param>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewModel">The viewmodel to bind to.</param>
        protected Binding(TView view, TControl control, TViewModel viewModel)
        {
            _view = view;
            Control = control;
            ViewModel = viewModel;
            Configuration = new BindingConfiguration();
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="view">The view to bind to.</param>
        /// <param name="control">The control to bind to.</param>
        /// <param name="viewProperty">The view property expression.</param>
        /// <param name="viewModel">The viewmodel to bind to.</param>
        /// <param name="viewModelProperty">The viewmodel property expression.</param>
        protected Binding(TView view,
                          TControl control,
                          Expression<Func<TControl, object>> viewProperty,
                          TViewModel viewModel,
                          Expression<Func<TViewModel, object>> viewModelProperty) : this(view, control, viewModel)
        {
            ViewModel = viewModel;
            ViewProperty = viewProperty;
            Control = control;
            ViewPropertyInfo = Control.GetPropertyInfo(ViewProperty);
            ViewModelProperty = viewModelProperty;
            ViewModelPropertyInfo = ViewModel.GetPropertyInfo(ViewModelProperty);
        }

        /// <summary>
        /// Gets the viewmodel this binding binds to.
        /// </summary>
        protected TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets the property expression specifying the target property.
        /// </summary>
        protected Expression<Func<TControl, object>> ViewProperty { get; private set; }

        /// <summary>
        /// Gets the property expression specifying the source property.
        /// </summary>
        protected Expression<Func<TViewModel, object>> ViewModelProperty { get; private set; }

        /// <summary>
        /// Gets the control this binding binds to.
        /// </summary>
        protected TControl Control { get; private set; }

        /// <summary>
        /// Gets the property info for the target property.
        /// </summary>
        protected PropertyInfo ViewPropertyInfo { get; private set; }

        /// <summary>
        /// Gets the property info for the source property.
        /// </summary>
        protected PropertyInfo ViewModelPropertyInfo { get; private set; }

        /// <summary>
        /// Checks whether this binding binds to the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to check.</param>
        /// <returns>True if this binding binds to the specified viewmodel, otherwise false.</returns>
        internal override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return ViewModel.Equals(viewModel);
        }

        /// <summary>
        /// Checks whether this vinding is affected by a change of the specified property in the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to check.</param>
        /// <param name="propertyName">The property to checks.</param>
        /// <returns>True if this binding binds to the specified viewmodel and the specified property, otherwise false.</returns>
        internal override bool IsAffectedBy(INotifyPropertyChanged viewModel, string propertyName)
        {
            return ViewModel.Equals(viewModel) && ViewModelPropertyInfo.Name == propertyName;
        }

        /// <summary>
        /// Gets the binding configuratiion.
        /// </summary>
        protected override BindingConfiguration Configuration { get; }

        /// <summary>
        /// checks whether this binding binds to the specified view.
        /// </summary>
        /// <param name="view">The view to check.</param>
        /// <returns>Ture if this binding binds to the specified view, otherwise false.</returns>
        internal override bool HasView(object view)
        {
            return View.Equals(view);
        }

        /// <summary>
        /// Propagates a new value from viewmodel to view.
        /// </summary>
        internal override void UpdateView()
        {
            var valueToSet = ViewModelPropertyInfo.GetValue(ViewModel);
            ViewPropertyInfo.SetValue(Control, valueToSet);
        }

        /// <summary>
        /// Unbinds this binding.
        /// </summary>
        internal override void Unbind()
        {
            _view = default(TView);
            ViewModel = default(TViewModel);
            ViewProperty = null;
            Control = default(TControl);
            ViewPropertyInfo = null;
            ViewModelProperty = null;
            ViewModelPropertyInfo = null;
        }

        /// <summary>
        /// Gets the view this binding binds to.
        /// </summary>
        internal override TView View
        {
            get
            {
                return _view;
            }
        }
    }
}