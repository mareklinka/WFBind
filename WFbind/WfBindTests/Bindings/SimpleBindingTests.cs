using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WFbind;
using WFbind.Tests;

namespace WfBindTests.Bindings
{
    [TestClass]
    public class SimpleBindingTests
    {
        [TestMethod]
        public void SimpleBindingTest_Form()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();
            
            var label = new Label {Text = initialText};
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form).Bind(label, _ => _.Text).To(vm, _ => _.Text);

            Assert.AreEqual(initialText, label.Text);

            vm.Text = newText;

            // assert
            Assert.AreEqual(newText, label.Text);
        }

        [TestMethod]
        public void SimpleBindingTest_UserControl()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var uc = new UserControl();

            var label = new Label { Text = initialText };
            uc.Controls.Add(label);

            BindingManager.Bind(uc).To(vm);

            // act
            BindingManager.For(uc).Bind(label, _ => _.Text).To(vm, _ => _.Text);

            Assert.AreEqual(initialText, label.Text);

            vm.Text = newText;

            // assert
            Assert.AreEqual(newText, label.Text);
        }
    }
}