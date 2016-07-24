using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WFbind;

namespace WFBind.Demo
{
    public partial class Form1 : Form
    {
        private readonly MyViewModel _myViewModel;

        public Form1()
        {
            _myViewModel = new MyViewModel();
            InitializeComponent();

            BindingManager.Bind(this).To(_myViewModel);
            BindingManager.For(this).Bind(label1, _ => _.Text).To(_myViewModel, vm => vm.Text);
            BindingManager.For(this)
                .Bind(textBox1, _ => _.Text)
                .To(_myViewModel, vm => vm.Text)
                .Setup(configuration => configuration.IsTwoWay = true)
                .Setup(configuration => configuration.UpdateSourceTrigger = UpdateSourceType.LostFocus);
            BindingManager.For(this)
                .Bind(textBox2, _ => _.Text)
                .To(_myViewModel, vm => vm.Text)
                .Setup(configuration => configuration.IsTwoWay = true)
                .Setup(configuration => configuration.UpdateSourceTrigger = UpdateSourceType.OnPropertyChanged);
            BindingManager.For(this)
                .Bind(textBox3, _ => _.Text)
                .To(_myViewModel, vm => vm.Text)
                .Setup(configuration => configuration.IsTwoWay = false);
            BindingManager.For(this).BindCommand(button2).To(_myViewModel, _ => _.Command);

            myControl1.Bind(_myViewModel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _myViewModel.Text = "AHAHAHA";
            _myViewModel.Command.CanExecuteProperty = !_myViewModel.Command.CanExecuteProperty;
        }
    }

    public class MyViewModel : INotifyPropertyChanged
    {
        private string _text = "Starting String";
        public event PropertyChangedEventHandler PropertyChanged;

        public MyViewModel()
        {
            Command = new MyCommand(this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public MyCommand Command { get; }

        public class MyCommand : ICommand
        {
            private readonly MyViewModel _vm;
            private bool _canExecuteProperty;

            public MyCommand(MyViewModel vm)
            {
                _vm = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecuteProperty
            {
                get
                {
                    return _canExecuteProperty;
                }

                set
                {
                    _canExecuteProperty = value;
                    OnCanExecuteChanged();
                }
            }

            public bool CanExecute()
            {
                return _canExecuteProperty;
            }

            public void Execute()
            {
                _vm.Text = "AA";
            }

            public void RaiseCanExecuteChanged()
            {
                OnCanExecuteChanged();
            }

            protected virtual void OnCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
