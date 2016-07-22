using System;
using System.ComponentModel;

namespace WFbind
{
    public abstract class FinalBinding
    {
        public FinalBinding Setup(Action<BindingConfiguration> setting)
        {
            setting(Configuration);
            return this;
        }

        internal abstract object View { get; }

        internal abstract bool HasViewModel(INotifyPropertyChanged viewModel);

        internal abstract void Unbind();

        internal abstract void Update();

        internal abstract bool IsAffectedBy(string propertyName);

        internal abstract BindingConfiguration Configuration { get; }
    }

    public sealed class FinalBinding<TView> : FinalBinding
    {
        private readonly BindingBuilder<TView> _source;

        public FinalBinding(BindingBuilder<TView> source)
        {
            _source = source;
        }

        internal override bool HasViewModel(INotifyPropertyChanged viewModel)
        {
            return _source.HasViewModel(viewModel);
        }

        internal override bool IsAffectedBy(string propertyName)
        {
            return _source.IsAffectedBy(propertyName);
        }

        internal override BindingConfiguration Configuration => _source.Configuration;

        internal override void Update()
        {
            _source.Update();
        }

        internal override void Unbind()
        {
            _source.Unbind();
        }

        internal override object View => _source.View;
    }
}