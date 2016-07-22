using System;
using System.ComponentModel;

namespace WFbind
{
    public sealed class ViewModelConfiguration<TView>
    {
        private readonly TView _view;

        public ViewModelConfiguration(TView view)
        {
            _view = view;
        }

        public void To<TViewModel>(TViewModel viewModel) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            BindingManager.AddViewModel(_view, viewModel);
        }
    }
}