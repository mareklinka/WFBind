using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace WFbind
{
    /// <summary>
    /// Binding manager.
    /// </summary>
    public static class BindingManager
    {
        private static readonly Dictionary<object, List<Binding>> Bindings =
            new Dictionary<object, List<Binding>>();

        private static readonly Dictionary<object, INotifyPropertyChanged> ViewModels =
           new Dictionary<object, INotifyPropertyChanged>();

        /// <summary>
        /// Configures viewmodel for the specified form.
        /// </summary>
        /// <param name="view">The view to set viewmodel for.</param>
        /// <exception cref="ArgumentNullException">Thrown when view is null.</exception>
        /// <returns>Viewmodel configuration.</returns>
        public static ViewModelConfiguration<Form> Bind(Form view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new ViewModelConfiguration<Form>(view);
        }

        /// <summary>
        /// Configures viewmodel for the specified usercontrol.
        /// </summary>
        /// <param name="view">The view to set viewmodel for.</param>
        /// <exception cref="ArgumentNullException">Thrown when view is null.</exception>
        /// <returns>Viewmodel configuration.</returns>
        public static ViewModelConfiguration<UserControl> Bind(UserControl view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }
            
            return new ViewModelConfiguration<UserControl>(view);
        }

        /// <summary>
        /// Configures a new binding on the specified form.
        /// </summary>
        /// <param name="view">The form to create a new binding for.</param>
        /// <exception cref="ArgumentNullException">Thrown when view is null.</exception>
        /// <returns>A binding builder.</returns>
        public static BindingBuilder<Form> For(Form view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new BindingBuilder<Form>(view);
        }

        /// <summary>
        /// Configures a new binding on the specified usercontrol.
        /// </summary>
        /// <param name="view">The form to create a new binding for.</param>
        /// <exception cref="ArgumentNullException">Thrown when view is null.</exception>
        /// <returns>A binding builder.</returns>
        public static BindingBuilder<UserControl> For(UserControl view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new BindingBuilder<UserControl>(view);
        }

        /// <summary>
        /// Adds a new binding.
        /// </summary>
        /// <typeparam name="TView">Type of the view the binding binds to.</typeparam>
        /// <param name="binding">The binding to add.</param>
        internal static void AddBinding<TView>(Binding<TView> binding)
        {
            List<Binding> config;
            if (Bindings.TryGetValue(binding.View, out config))
            {
                config.Add(binding);
            }
            else
            {
                Bindings.Add(binding.View, new List<Binding> {binding});
            }  

            binding.UpdateView();
        }

        /// <summary>
        /// Adds a new viewmodel.
        /// </summary>
        /// <typeparam name="TView">Type of view.</typeparam>
        /// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
        /// <param name="view">The view the viewmodel binds to.</param>
        /// <param name="viewModel">The viewmodel to add.</param>
        internal static void AddViewModel<TView, TViewModel>(TView view, TViewModel viewModel) where TViewModel : INotifyPropertyChanged
        {
            INotifyPropertyChanged oldViewModel;

            if (ViewModels.TryGetValue(view, out oldViewModel))
            {
                // TODO: rebind?
                Unbind(view, oldViewModel);
                ViewModels[view] = viewModel;
            }
            else
            {
                ViewModels.Add(view, viewModel);
            }

            HookHandler(viewModel);
        }

        /// <summary>
        /// Hooks the OnPropertyChanged event.
        /// </summary>
        /// <param name="viewModel">The viewmodel to hook.</param>
        private static void HookHandler(INotifyPropertyChanged viewModel)
        {
            // prevent multiple subscription
            viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Unbinds the specified view from the viewmodel.
        /// </summary>
        /// <param name="view">The view to unbind.</param>
        /// <param name="viewModel">The viewmodel to unbind.</param>
        private static void Unbind(object view, INotifyPropertyChanged viewModel)
        {
            viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            var bindingsToRemove = Bindings.Values.SelectMany(_ => _).Where(_ => _.HasView(view) && _.HasViewModel(viewModel)).ToList();

            foreach (var binding in bindingsToRemove)
            {
                binding.Unbind();
                List<Binding> list;
                
                if (Bindings.TryGetValue(view, out list))
                {
                    Bindings[view].Remove(binding);

                    if (list.Count == 0)
                    {
                        Bindings.Remove(view);
                    }
                }
            }
        }

        /// <summary>
        /// Unbinds the specified form.
        /// </summary>
        /// <param name="view">The form to unbind.</param>
        public static void Unbind(Form view)
        {
            UnbindView(view);
        }

        /// <summary>
        /// Unbinds the specified usercontrol.
        /// </summary>
        /// <param name="view">The form to usercontrol.</param>
        public static void Unbind(UserControl view)
        {
            UnbindView(view);
        }

        /// <summary>
        /// Unbinds the specified view.
        /// </summary>
        /// <param name="view">The view to unbind.</param>
        /// <exception cref="ArgumentNullException">Thrown when view is null.</exception>
        private static void UnbindView(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var viewModel = ViewModels[view];
            ViewModels.Remove(view);

            if (ViewModels.Values.All(_ => _ != viewModel))
            {
                // no other view binds to this viewmodel
                viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            }

            var bindingsToRemove = Bindings.Values.SelectMany(_ => _).Where(_ => _.HasView(view)).ToList();

            foreach (var binding in bindingsToRemove)
            {
                binding.Unbind();
                Bindings[view].Remove(binding);
            }

            List<Binding> list;

            if (Bindings.TryGetValue(view, out list) && list.Count == 0)
            {
                Bindings.Remove(view);
            }
        }

        /// <summary>
        /// Handles property changes in the viewmodel.
        /// </summary>
        private static void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var views = GetViewsFor((INotifyPropertyChanged) sender);

            foreach (var view in views)
            {
                List<Binding> list;

                if (Bindings.TryGetValue(view, out list))
                {
                    var viewModel = ViewModels[view];
                    var bindingsToUpdate = list.Where(b => b.IsAffectedBy(viewModel, e.PropertyName));

                    foreach (var bindingConfiguration in bindingsToUpdate)
                    {
                        bindingConfiguration.UpdateView();
                    }
                }
            }
        }

        /// <summary>
        /// Gets viewmodel for the specified view.
        /// </summary>
        /// <typeparam name="TViewModel">Type of viewmodel to retrieve.</typeparam>
        /// <param name="view">The view to retrieve viewmodel for.</param>
        /// <returns>Viewmodel bound to the specified view.</returns>
        internal static TViewModel GetViewModelFor<TViewModel>(object view) where TViewModel : INotifyPropertyChanged
        {
            return (TViewModel)ViewModels[view];
        }

        /// <summary>
        /// Gets views for the specified viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to retrieve views for.</param>
        /// <returns>A collection of views.</returns>
        internal static ICollection<object> GetViewsFor(INotifyPropertyChanged viewModel)
        {
            return new ReadOnlyCollection<object>(ViewModels.Where(_ => _.Value == viewModel).Select(_ => _.Key).ToList());
        }
    }
}
