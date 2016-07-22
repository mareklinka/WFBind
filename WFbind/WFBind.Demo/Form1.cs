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
            InitializeComponent();

            _myViewModel = new MyViewModel();
            this.Bind().To(_myViewModel);
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
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            _myViewModel.Text = "AHAHAHA";
        }
    }

    public class MyViewModel : INotifyPropertyChanged
    {
        private string _text = "Starting String";
        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
