using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFbind
{
    public abstract class AbstractBindingBuilder
    {
        public static FinalBinding<TView> Build<TView, TControl, TViewModel>(TView view,
            TControl control, Expression<Func<TControl, object>> viewProperty, TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (typeof(TControl) == typeof(Label))
            {
                return new FinalBinding<TView>(new LabelBinding<TView, TViewModel>(view, control as Label,
                    viewProperty as Expression<Func<Label, object>>, viewModel, viewModelProperty));
            }

            if (typeof(TControl) == typeof(TextBox))
            {
                return new FinalBinding<TView>(new TextBoxBinding<TView, TViewModel>(view, control as TextBox,
                    viewProperty as Expression<Func<TextBox, object>>, viewModel, viewModelProperty));
            }

            throw new NotImplementedException();
        }

        protected AbstractBindingBuilder()
        {
            Configuration = new BindingConfiguration();
        }

        internal BindingConfiguration Configuration { get; }

        protected virtual void UpdateSource()
        {
        }

        internal abstract bool HasViewModel(INotifyPropertyChanged viewModel);

        internal abstract void Unbind();

        internal abstract void Update();

        internal abstract bool IsAffectedBy(string propertyName);
    }
}