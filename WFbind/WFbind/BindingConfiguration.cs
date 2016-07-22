namespace WFbind
{
    public class BindingConfiguration
    {
        public bool IsTwoWay { get; set; } = false;

        public UpdateSourceType UpdateSourceTrigger { get; set; } = UpdateSourceType.OnPropertyChanged;
    }

    public enum UpdateSourceType
    {
        OnPropertyChanged,
        LostFocus
    }
}