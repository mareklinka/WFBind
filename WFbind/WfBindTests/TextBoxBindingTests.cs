using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WFbind.Tests
{
    [TestClass]
    public class TextBoxBindingTests
    {
        [TestMethod]
        public void BindingToView_BasicTest_Form()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form).Bind(tb, _ => _.Text).To(vm, _ => _.Text);

            Assert.AreEqual(initialText, tb.Text);

            vm.Text = newText;

            // assert
            Assert.AreEqual(newText, tb.Text);
        }

        [TestMethod]
        public void BindingToView_BasicTest_UserControl()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new UserControl();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form).Bind(tb, _ => _.Text).To(vm, _ => _.Text);

            Assert.AreEqual(initialText, tb.Text);

            vm.Text = newText;

            // assert
            Assert.AreEqual(newText, tb.Text);
        }


        [TestMethod]
        public void BindingToViewModel_WithoutTwoWay_NoChange()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(tb, _ => _.Text)
                .To(vm, _ => _.Text)
                .Setup(_ => _.IsTwoWay = false);

            Assert.AreEqual(initialText, tb.Text);

            tb.Text = newText;

            // assert
            Assert.AreEqual(initialText, vm.Text);
        }

        [TestMethod]
        public void BindingToViewModel_WithTwoWay_OnTextChange()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(tb, _ => _.Text)
                .To(vm, _ => _.Text)
                .Setup(_ => _.IsTwoWay = true)
                .Setup(_ => _.UpdateSourceTrigger = UpdateSourceType.OnPropertyChanged);

            Assert.AreEqual(initialText, tb.Text);

            tb.Text = newText;

            // assert
            Assert.AreEqual(newText, vm.Text);
        }

        [TestMethod]
        public void BindingToViewModel_WithTwoWay_OnLostFocus_WithoutFocusChange()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(tb, _ => _.Text)
                .To(vm, _ => _.Text)
                .Setup(_ => _.IsTwoWay = true)
                .Setup(_ => _.UpdateSourceTrigger = UpdateSourceType.LostFocus);

            Assert.AreEqual(initialText, tb.Text);

            tb.Text = newText;

            // assert
            Assert.AreEqual(initialText, vm.Text);
        }

        [TestMethod]
        public void BindingToViewModel_WithTwoWay_OnLostFocus_WithFocusChange()
        {
            // arrange
            const string initialText = "Initial text";
            const string newText = "New text";

            var vm = new TestingViewModel { Text = initialText };
            var form = new Form();

            var tb = new TextBox { Text = initialText };
            form.Controls.Add(tb);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(tb, _ => _.Text)
                .To(vm, _ => _.Text)
                .Setup(_ => _.IsTwoWay = true)
                .Setup(_ => _.UpdateSourceTrigger = UpdateSourceType.LostFocus);

            Assert.AreEqual(initialText, tb.Text);

            tb.Text = newText;
            tb.FireEvent(nameof(tb.LostFocus), EventArgs.Empty);

            // assert
            Assert.AreEqual(newText, vm.Text);
        }

        [TestMethod]
        public void Unbind_Unhooks()
        {
            const string Text = "test";

            var vm1 = new TestingViewModel();
            var form = new Form();

            var label = new TextBox();
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
    }
}