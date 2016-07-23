using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFbind;
using WFBind.Exceptions;

namespace WFBind.Bindings
{
    public class RadioBinding<TView, TViewModel> : TwoWayBinding<TView, RadioButton, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public RadioBinding(TView view, RadioButton control, Expression<Func<RadioButton, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            if (ViewPropertyInfo.Name != "Checked")
            {
                throw new InvalidBindingException("RadioBinding can only be used to bind to RadioBinding.Checked");
            }

            HookEvents(control);
        }

        private void HookEvents(RadioButton control)
        {
            control.CheckedChanged += ControlOnCheckedChanged;
        }

        private void ControlOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            UpdateViewModel();
        }

        internal override void UpdateViewModel()
        {
            if (Configuration.IsTwoWay)
            {
                base.UpdateViewModel();
            }
        }

        private void UnhookEvents(RadioButton control)
        {
            control.CheckedChanged -= ControlOnCheckedChanged;
        }

        internal override void Unbind()
        {
            UnhookEvents(Control);
            base.Unbind();
        }
    }
}