using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFbind;
using WFBind.Exceptions;

namespace WFBind.Bindings
{
    internal class TextBoxBinding<TView, TViewModel> : TwoWayBinding<TView, TextBox, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public TextBoxBinding(TView view, TextBox control, Expression<Func<TextBox, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            if (ViewPropertyInfo.Name != "Text")
            {
                throw new InvalidBindingException("TextBoxBinding can only be used to bind to TextBox.Text");
            }

            HookEvents(control);
        }

        private void HookEvents(TextBox control)
        {
            control.TextChanged += ControlOnTextChanged;
            control.LostFocus += ControlOnLostFocus;
        }

        private void ControlOnLostFocus(object sender, EventArgs eventArgs)
        {
            if (Configuration.UpdateSourceTrigger == UpdateSourceType.LostFocus)
            {
                UpdateViewModel();
            }
        }

        protected override void UpdateViewModel()
        {
            if (Configuration.IsTwoWay)
            {
                base.UpdateViewModel();
            }
        }

        private void UnhookEvents(TextBox control)
        {
            control.TextChanged -= ControlOnTextChanged;
            control.LostFocus -= ControlOnLostFocus;
        }

        private void ControlOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (Configuration.UpdateSourceTrigger == UpdateSourceType.OnPropertyChanged)
            {
                UpdateViewModel();
            }
        }

        internal override void Unbind()
        {
            UnhookEvents(Control);
            base.Unbind();
        }
    }
}