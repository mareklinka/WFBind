using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace WFbind
{
    /// <summary>
    /// Viewmodel configuration.
    /// </summary>
    /// <typeparam name="TView">The type of view to configure viewmodel for.</typeparam>
    public sealed class ViewModelConfiguration<TView>
    {
        private readonly TView _view;

        /// <summary>
        /// Creates a new instance of the ViewModelConfiguration class.
        /// </summary>
        /// <param name="view"></param>
        public ViewModelConfiguration(TView view)
        {
            _view = view;
        }

        /// <summary>
        /// Sets the viewmodel for the view.
        /// </summary>
        /// <typeparam name="TViewModel">Type of viewmodel.</typeparam>
        /// <param name="viewModel">Viewmodel to set.</param>
        /// <exception cref="ArgumentNullException">thrown when viewModel is null.</exception>
        public void To<TViewModel>(TViewModel viewModel) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var form = _view as Form;

            // hook the FormClosed event to perform automatic unbinding
            if (form != null)
            {
                form.FormClosed += (sender, args) => BindingManager.Unbind(form);
            }

            BindingManager.AddViewModel(_view, viewModel);
        }
    }
}