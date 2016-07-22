using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFbind
{
    public abstract class AbstractBinding
    {
        public static Binding<TView> Build<TView, TControl, TViewModel>(TView view,
            TControl control, Expression<Func<TControl, object>> viewProperty, TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (typeof(TControl) == typeof(Label))
            {
                return new LabelBinding<TView, TViewModel>(view, control as Label,
                    viewProperty as Expression<Func<Label, object>>, viewModel, viewModelProperty);
            }

            if (typeof(TControl) == typeof(TextBox))
            {
                return new TextBoxBinding<TView, TViewModel>(view, control as TextBox,
                    viewProperty as Expression<Func<TextBox, object>>, viewModel, viewModelProperty);
            }

            throw new NotImplementedException();
        }

        protected virtual void UpdateSource()
        {
        }

        protected AbstractBinding()
        {
            Configuration = new BindingConfiguration();
        }

        internal BindingConfiguration Configuration { get; }

        public AbstractBinding Setup(Action<BindingConfiguration> setting)
        {
            setting(Configuration);
            return this;
        }

        internal abstract bool IsAffectedBy(string propertyName);

        internal abstract void Update();

        internal abstract void Unbind();

        internal abstract bool HasViewModel(INotifyPropertyChanged viewModel);
    }
}