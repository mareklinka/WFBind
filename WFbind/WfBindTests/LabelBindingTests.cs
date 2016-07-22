using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WFbind.Tests
{
    [TestClass]
    public class LabelBindingTests
    {
        [TestMethod]
        public void LabelBindingTest()
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
    }
}