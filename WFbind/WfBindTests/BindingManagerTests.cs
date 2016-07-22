using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WFbind.Tests
{
    [TestClass]
    public class BindingManagerTests
    {
        [TestMethod]
        public void ReplaceViewModel_UnbindsOldViewModel()
        {
            const string Text = "test";

            var vm1 = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm1);

            // act
            BindingManager.For(form).Bind(label, _ => _.Text).To(vm1, _ => _.Text);

            vm1.Text = Text;
            Assert.AreEqual(Text, label.Text);

            var vm2 = new TestingViewModel();
            BindingManager.Bind(form).To(vm2);

            vm2.Text = Text + "abc";
            Assert.AreEqual(Text, label.Text);
        }

        [TestMethod]
        public void ReplaceViewModel_MultipleBindingsToSingleProperty()
        {
            const string InitialText = "Text";
            const string NewText = "Text 2";

            var vm1 = new TestingViewModel { Text = InitialText };
            var form = new Form();

            var label1 = new Label();
            var label2 = new Label();
            form.Controls.Add(label2);

            BindingManager.Bind(form).To(vm1);

            // act
            BindingManager.For(form).Bind(label1, _ => _.Text).To(vm1, _ => _.Text);
            BindingManager.For(form).Bind(label2, _ => _.Text).To(vm1, _ => _.Text);

            vm1.Text = NewText;

            Assert.AreEqual(label1.Text, NewText);
            Assert.AreEqual(label2.Text, NewText);
        }
    }
}