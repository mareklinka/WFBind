using System.ComponentModel;
using WFBind;

namespace WfBindTests
{
    public interface ITestingViewModel : INotifyPropertyChanged
    {
        ICommand Command { get; }

        bool WasCommandCalled { get; set; }
    }
}