using System.Windows.Forms;
using WFbind;

namespace WFBind.Demo
{
    public partial class MyControl : UserControl
    {
        private MyViewModel _viewModel;

        public MyControl()
        {
            InitializeComponent();
        }

        public void Bind(MyViewModel viewModel)
        {
            _viewModel = viewModel;
            this.Bind().To(_viewModel);
            BindingManager.For(this)
                .Bind(label1, _ => _.Text)
                .To(_viewModel, vm => vm.Text);
        }
    }
}
