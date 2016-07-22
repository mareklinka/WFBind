using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace WFbind
{
    public class TextBoxBinding<TView, TViewModel> : TwoWayBinding<TView, TextBox, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public TextBoxBinding(TView view, TextBox control, Expression<Func<TextBox, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            HookEvents(Control);
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
                UpdateSource();
            }
        }

        protected override void UpdateSource()
        {
            if (Configuration.IsTwoWay &&
                Control.GetPropertyInfo(ViewProperty).Name == "Text")
            {
                base.UpdateSource();
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
                UpdateSource();
            }
        }

        internal override void Unbind()
        {
            base.Unbind();
            UnhookEvents(Control);
        }
    }
}