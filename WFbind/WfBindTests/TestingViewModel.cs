using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WfBindTests
{
    public class TestingViewModel : INotifyPropertyChanged
    {
        private string _text;
        private bool _bool;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public bool BoolValue
        {
            get
            {
                return _bool;
            }

            set
            {
                _bool = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}