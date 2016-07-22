using System;
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Bind_NullView_Throws()
        {
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind<Form>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Bind_NullViewModel_Throws()
        {
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To<TestingViewModel>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void For_NullView_Throws()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);
            BindingManager.For<Form>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void For_NullViewProperty_Throws()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind(label, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void For_NullControl_Throws()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind<Label>(null, _ => _.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void For_NullViewModelProperty_Throws()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind(label, _ => _.Text).To(vm, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void For_NullViewModel_Throws()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var label = new Label();
            form.Controls.Add(label);

            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind(label, _ => _.Text).To<TestingViewModel>(null, _ => _.Text);
        }
    }
}