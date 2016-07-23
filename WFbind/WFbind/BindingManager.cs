using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace WFbind
{
    public static class BindingManager
    {
        private static readonly Dictionary<object, List<Binding>> _bindings =
            new Dictionary<object, List<Binding>>();

        private static readonly Dictionary<object, INotifyPropertyChanged> _viewModels =
           new Dictionary<object, INotifyPropertyChanged>();

        public static ViewModelConfiguration<Form> Bind(Form view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            view.FormClosed += (sender, args) => Unbind(view);
            return new ViewModelConfiguration<Form>(view);
        }

        public static ViewModelConfiguration<UserControl> Bind(UserControl view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }
            
            return new ViewModelConfiguration<UserControl>(view);
        }

        public static BindingBuilder<Form> For(Form view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new BindingBuilder<Form>(view);
        }

        public static BindingBuilder<UserControl> For(UserControl view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new BindingBuilder<UserControl>(view);
        }

        internal static void AddBinding<TView>(Binding<TView> binding)
        {
            List<Binding> config;
            if (_bindings.TryGetValue(binding.View, out config))
            {
                config.Add(binding);
            }
            else
            {
                _bindings.Add(binding.View, new List<Binding> {binding});
            }  

            binding.UpdateView();
        }

        internal static void AddViewModel<TView, TViewModel>(TView view, TViewModel viewModel) where TViewModel : INotifyPropertyChanged
        {
            INotifyPropertyChanged oldViewModel;

            if (_viewModels.TryGetValue(view, out oldViewModel))
            {
                Unbind(view, oldViewModel);
                _viewModels[view] = viewModel;
            }
            else
            {
                _viewModels.Add(view, viewModel);
            }

            HookHandler(viewModel);
        }

        private static void HookHandler(INotifyPropertyChanged viewModel)
        {
            // prevent multiple subscription
            viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private static void Unbind(object view, INotifyPropertyChanged viewModel)
        {
            viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            var bindingsToRemove = _bindings.Values.SelectMany(_ => _).Where(_ => _.HasView(view) && _.HasViewModel(viewModel)).ToList();

            foreach (var binding in bindingsToRemove)
            {
                binding.Unbind();
                List<Binding> list;
                
                if (_bindings.TryGetValue(view, out list))
                {
                    _bindings[view].Remove(binding);

                    if (list.Count == 0)
                    {
                        _bindings.Remove(view);
                    }
                }
            }
        }

        public static void Unbind(Form view)
        {
            UnbindView(view);
        }

        public static void Unbind(UserControl view)
        {
            UnbindView(view);
        }

        private static void UnbindView(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var viewModel = _viewModels[view];
            _viewModels.Remove(view);

            if (_viewModels.Values.All(_ => _ != viewModel))
            {
                // no other view binds to this viewmodel
                viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            }

            var bindingsToRemove = _bindings.Values.SelectMany(_ => _).Where(_ => _.HasView(view)).ToList();

            foreach (var binding in bindingsToRemove)
            {
                binding.Unbind();
                _bindings[view].Remove(binding);
            }

            List<Binding> list;

            if (_bindings.TryGetValue(view, out list) && list.Count == 0)
            {
                _bindings.Remove(view);
            }
        }

        private static void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var views = GetViewsFor((INotifyPropertyChanged) sender);

            foreach (var view in views)
            {
                List<Binding> list;

                if (_bindings.TryGetValue(view, out list))
                {
                    var viewModel = _viewModels[view];
                    var bindingsToUpdate = list.Where(b => b.IsAffectedBy(viewModel, e.PropertyName));

                    foreach (var bindingConfiguration in bindingsToUpdate)
                    {
                        bindingConfiguration.UpdateView();
                    }
                }
            }
        }

        internal static TViewModel GetViewModelFor<TViewModel>(object view) where TViewModel : INotifyPropertyChanged
        {
            return (TViewModel)_viewModels[view];
        }

        internal static IEnumerable<object> GetViewsFor(INotifyPropertyChanged viewModel)
        {
            return _viewModels.Where(_ => _.Value == viewModel).Select(_ => _.Key);
        }
    }
}
