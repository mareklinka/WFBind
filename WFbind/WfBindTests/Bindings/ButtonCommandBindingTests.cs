using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WFbind;
using WFbind.Tests;
using WFBind;

namespace WfBindTests.Bindings
{
    [TestClass]
    public class ButtonCommandBindingTests
    {
        [TestMethod]
        public void BasicTest()
        {
            // arrange
            var vmMock = new Mock<ITestingViewModel>();
            vmMock.SetupProperty(_ => _.WasCommandCalled);

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(_ => _.Execute()).Callback(() => vmMock.Object.WasCommandCalled = true);
            commandMock.Setup(_ => _.CanExecute()).Returns(true);

            vmMock.Setup(_ => _.Command).Returns(commandMock.Object);

            var form = new Form();
            var button = new Button();
            form.Controls.Add(button);

            BindingManager.Bind(form).To(vmMock.Object);
            BindingManager.For(form).BindCommand(button).To(vmMock.Object, _ => _.Command);

            // act
            button.FireEvent("Click", EventArgs.Empty);

            // assert
            vmMock.VerifySet(foo => foo.WasCommandCalled = true);
        }

        [TestMethod]
        public void Unbind_Unhooks()
        {
            var vmMock = new Mock<ITestingViewModel>();

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(_ => _.Execute()).Callback(() => vmMock.Object.WasCommandCalled = true);
            commandMock.Setup(_ => _.CanExecute()).Returns(true);

            vmMock.Setup(_ => _.Command).Returns(commandMock.Object);

            var form = new Form();

            var control = new Button();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vmMock.Object);
            BindingManager.For(form).BindCommand(control).To(vmMock.Object, _ => _.Command);
            control.FireEvent("Click", EventArgs.Empty);

            var vmMock2 = new Mock<ITestingViewModel>();

            var commandMock2 = new Mock<ICommand>();
            commandMock2.Setup(_ => _.Execute()).Callback(() => vmMock2.Object.WasCommandCalled = true);
            commandMock2.Setup(_ => _.CanExecute()).Returns(true);

            BindingManager.Bind(form).To(vmMock2.Object);
            control.FireEvent("Click", EventArgs.Empty);

            vmMock.VerifySet(_ => _.WasCommandCalled = true, Times.Once);
            vmMock2.VerifySet(_ => _.WasCommandCalled = true, Times.Never);
        }

        [TestMethod]
        public void CanExecute_ChangesEnabledState()
        {
            // arrange
            var vmMock = new Mock<ITestingViewModel>();

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(_ => _.CanExecute()).Returns(false);
            
            vmMock.Setup(_ => _.Command).Returns(commandMock.Object);

            var form = new Form();
            var button = new Button();
            form.Controls.Add(button);

            Assert.IsTrue(button.Enabled);

            BindingManager.Bind(form).To(vmMock.Object);
            BindingManager.For(form).BindCommand(button).To(vmMock.Object, _ => _.Command);

            Assert.IsFalse(button.Enabled);

            commandMock.Setup(_ => _.CanExecute()).Returns(true);
            commandMock.Raise(_ => _.CanExecuteChanged += null, EventArgs.Empty);

            Assert.IsTrue(button.Enabled);
        }
    }
}