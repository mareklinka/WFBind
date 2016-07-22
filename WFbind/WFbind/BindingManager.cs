using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace WFbind
{
    public static class BindingManager
    {
        private static readonly Dictionary<object, List<AbstractBinding>> _bindings =
            new Dictionary<object, List<AbstractBinding>>();

        private static readonly Dictionary<object, INotifyPropertyChanged> _viewModels =
           new Dictionary<object, INotifyPropertyChanged>();

        public static ViewModelConfiguration<TView> Bind<TView>(TView view) where TView : Form
        {
            return new ViewModelConfiguration<TView>(view);
        }

        public static Binding<TView> For<TView>(TView view) where TView : Form
        {
            return new Binding<TView>(view);
        }

        internal static void AddBinding<TView>(Binding<TView> binding)
        {
            List<AbstractBinding> config;
            if (_bindings.TryGetValue(binding.View, out config))
            {
                config.Add(binding);
            }
            else
            {
                _bindings.Add(binding.View, new List<AbstractBinding> {binding});
            }  

            binding.Update();
        }

        internal static void AddViewModel<TView, TViewModel>(TView view, TViewModel viewModel) where TViewModel : INotifyPropertyChanged
        {
            INotifyPropertyChanged oldViewModel;

            if (_viewModels.TryGetValue(view, out oldViewModel))
            {
                Unbind(oldViewModel);
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
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private static void Unbind(INotifyPropertyChanged viewModel)
        {
            viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            var bindingsToRemove = _bindings.Values.SelectMany(_ => _).Where(_ => _.HasViewModel(viewModel)).ToList();
            var view = GetViewFor(viewModel);

            foreach (var binding in bindingsToRemove)
            {
                binding.Unbind();
                _bindings[view].Remove(binding);
            }
        }

        private static void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var view = GetViewFor((INotifyPropertyChanged)sender);
            var bindings = _bindings[view];
            var bindingsToUpdate = bindings.Where(b => b.IsAffectedBy(e.PropertyName));

            foreach (var bindingConfiguration in bindingsToUpdate)
            {
                bindingConfiguration.Update();
            }
        }

        internal static TViewModel GetViewModelFor<TViewModel>(object view) where TViewModel : INotifyPropertyChanged
        {
            return (TViewModel)_viewModels[view];
        }

        internal static object GetViewFor(INotifyPropertyChanged viewModel)
        {
            return _viewModels.Single(_ => _.Value == viewModel).Key;
        }
    }
}
