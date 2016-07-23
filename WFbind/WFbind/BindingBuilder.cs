using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using WFBind;

namespace WFbind
{
    public class BindingBuilder<TView> : AbstractBindingBuilder
    {
        internal TView View { get; }

        internal BindingBuilder(TView view)
        {
           View = view;
        }

        public BindingBuilder<TView, TControl> Bind<TControl>(TControl control, Expression<Func<TControl, object>> viewProperty)
        {
            return new BindingBuilder<TView, TControl>(View, control, viewProperty);
        }

        public CommandBindingBuilder<TView, Button> BindCommand(Button button)
        {
            return new CommandBindingBuilder<TView, Button>(View, button);
        }
    }

    public class BindingBuilder<TView, TControl> : BindingBuilder<TView>
    {
        public TControl Control { get; }
        public Expression<Func<TControl, object>> ViewProperty { get; }

        internal BindingBuilder(TView view, TControl control, Expression<Func<TControl, object>> viewProperty) : base(view)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            Control = control;
            ViewProperty = viewProperty;
        }

        public Binding To<TViewModel>(TViewModel viewModel, Expression<Func<TViewModel, object>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty == null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            var binding = Build(View, Control, ViewProperty, viewModel, viewModelProperty);

            BindingManager.AddBinding(binding);

            return binding;
        }
    }

    public class CommandBindingBuilder<TView, TControl> : BindingBuilder<TView>
    {
        internal TControl Control { get; }

        internal CommandBindingBuilder(TView view, TControl control) : base(view)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Control = control;
        }

        public Binding To<TViewModel>(TViewModel viewModel, Expression<Func<TViewModel, ICommand>> viewModelProperty) where TViewModel : INotifyPropertyChanged
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty == null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            var binding = BuildCommand(View, Control, viewModel, viewModelProperty);

            BindingManager.AddBinding(binding);

            return binding;
        }
    }
}