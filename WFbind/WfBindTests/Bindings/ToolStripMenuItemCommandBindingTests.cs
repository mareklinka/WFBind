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
    public class ToolStripMenuItemCommandBindingTests
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
            var menuStrip = new MenuStrip();
            var menu = new ToolStripMenuItem();
            menuStrip.Items.Add(menu);
            form.MainMenuStrip = menuStrip;

            BindingManager.Bind(form).To(vmMock.Object);
            BindingManager.For(form).BindCommand(menu).To(vmMock.Object, _ => _.Command);

            // act
            menu.FireEvent("Click", EventArgs.Empty);

            // assert
            vmMock.VerifySet(foo => foo.WasCommandCalled = true, Times.Once);
        }

        [TestMethod]
        public void Unbind_Unhooks()
        {
            var vmMock = new Mock<ITestingViewModel>();
            vmMock.SetupProperty(_ => _.WasCommandCalled);

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(_ => _.Execute()).Callback(() => vmMock.Object.WasCommandCalled = true);
            commandMock.Setup(_ => _.CanExecute()).Returns(true);

            vmMock.Setup(_ => _.Command).Returns(commandMock.Object);

            var form = new Form();
            var menuStrip = new MenuStrip();
            var menu = new ToolStripMenuItem();
            menuStrip.Items.Add(menu);
            form.MainMenuStrip = menuStrip;

            BindingManager.Bind(form).To(vmMock.Object);

            BindingManager.For(form).BindCommand(menu).To(vmMock.Object, _ => _.Command);
            menu.FireEvent("Click", EventArgs.Empty);

            var vmMock2 = new Mock<ITestingViewModel>();
            vmMock2.SetupProperty(_ => _.WasCommandCalled);

            var commandMock2 = new Mock<ICommand>();
            commandMock2.Setup(_ => _.Execute()).Callback(() => vmMock2.Object.WasCommandCalled = true);
            commandMock2.Setup(_ => _.CanExecute()).Returns(true);

            BindingManager.Bind(form).To(vmMock2.Object);

            menu.FireEvent("Click", EventArgs.Empty);

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
            var menuStrip = new MenuStrip();
            var menu = new ToolStripMenuItem();
            menuStrip.Items.Add(menu);
            form.MainMenuStrip = menuStrip;

            Assert.IsTrue(menu.Enabled);

            BindingManager.Bind(form).To(vmMock.Object);
            BindingManager.For(form).BindCommand(menu).To(vmMock.Object, _ => _.Command);

            Assert.IsFalse(menu.Enabled);

            commandMock.Setup(_ => _.CanExecute()).Returns(true);
            commandMock.Raise(_ => _.CanExecuteChanged += null, EventArgs.Empty);

            Assert.IsTrue(menu.Enabled);
        }
    }
}