using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFbind;
using WFBind.Exceptions;

namespace WFBind.Bindings
{
    internal class CheckBoxBinding<TView, TViewModel> : TwoWayBinding<TView, CheckBox, TViewModel>
        where TViewModel : INotifyPropertyChanged
    {
        public CheckBoxBinding(TView view, CheckBox control, Expression<Func<CheckBox, object>> viewProperty,
            TViewModel viewModel,
            Expression<Func<TViewModel, object>> viewModelProperty)
            : base(view, control, viewProperty, viewModel, viewModelProperty)
        {
            if (ViewPropertyInfo.Name != "Checked")
            {
                throw new InvalidBindingException("CheckBoxBinding can only be used to bind to CheckBox.Checked");
            }

            HookEvents(control);
        }

        private void HookEvents(CheckBox control)
        {
            control.CheckedChanged += ControlOnCheckedChanged;
        }

        private void ControlOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            UpdateViewModel();
        }

        protected override void UpdateViewModel()
        {
            if (Configuration.IsTwoWay)
            {
                base.UpdateViewModel();
            }
        }

        private void UnhookEvents(CheckBox control)
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