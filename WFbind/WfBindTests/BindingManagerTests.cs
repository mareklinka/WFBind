using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WfBindTests;

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

            BindingManager.Bind((Form)null);
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
            BindingManager.For((Form)null);
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

        [TestMethod]
        public void GC_Test_NoBinding()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var weakVm = new WeakReference<TestingViewModel>(vm);
            var weakForm = new WeakReference<Form>(form);

            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));

            BindingManager.Bind(form).To(vm);

            vm = null;
            form = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
            Debug.WriteLine(vm); // to prevent optimization
            Debug.WriteLine(form); // to prevent optimization

            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));

            BindingManager.Unbind(form);
            vm = null;
            form = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
            Debug.WriteLine(vm); // to prevent optimization
            Debug.WriteLine(form); // to prevent optimization

            Assert.IsFalse(weakForm.TryGetTarget(out form));
            Assert.IsFalse(weakVm.TryGetTarget(out vm));
        }

        /// <summary>
        /// Tests whether LebelBinding properly releases all references after Unbind is called.
        /// </summary>
        [TestMethod]
        public void GC_Test_LabelBinding()
        {
            // arrange
            // setup hard references
            var vm = new TestingViewModel();
            var form = new Form();
            var label = new Label();
            form.Controls.Add(label);

            // setup weak references
            var weakVm = new WeakReference<TestingViewModel>(vm);
            var weakForm = new WeakReference<Form>(form);
            var weakLabel =new WeakReference<Label>(label);

            // smoke test - all weak references have targets here
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));
            Assert.IsTrue(weakLabel.TryGetTarget(out label));

            // setup label binding
            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind(label, _ => _.Text).To(vm, _ => _.Text);

            ClearReferences(ref vm, ref form, ref label);

            GC.Collect(2, GCCollectionMode.Forced, true);
            WriteReferences(vm, form, label);

            // smoke test - all references are still valid here, binding manager holds references
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));
            Assert.IsTrue(weakLabel.TryGetTarget(out label));

            // act
            BindingManager.Unbind(form);

            ClearReferences(ref vm, ref form, ref label);
            
            GC.Collect(2, GCCollectionMode.Forced, true);
            WriteReferences(vm, form, label);

            // assert - all weak references will be null if Unbind cleaned up properly
            Assert.IsFalse(weakForm.TryGetTarget(out form));
            Assert.IsFalse(weakVm.TryGetTarget(out vm));
            Assert.IsFalse(weakLabel.TryGetTarget(out label));
        }

        /// <summary>
        /// Tests whether LebelBinding properly releases all references after Unbind is called.
        /// </summary>
        [TestMethod]
        public void GC_Test_TextBinding()
        {
            // arrange
            // setup hard references
            var vm = new TestingViewModel();
            var form = new Form();
            var textbox = new TextBox();
            form.Controls.Add(textbox);

            // setup weak references
            var weakVm = new WeakReference<TestingViewModel>(vm);
            var weakForm = new WeakReference<Form>(form);
            var weakTextbox = new WeakReference<TextBox>(textbox);

            // smoke test - all weak references have targets here
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));
            Assert.IsTrue(weakTextbox.TryGetTarget(out textbox));

            // setup label binding
            BindingManager.Bind(form).To(vm);
            BindingManager.For(form).Bind(textbox, _ => _.Text).To(vm, _ => _.Text);

            ClearReferences(ref vm, ref form, ref textbox);

            GC.Collect(2, GCCollectionMode.Forced, true);
            WriteReferences(vm, form, textbox);

            // smoke test - all references are still valid here, binding manager holds references
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));
            Assert.IsTrue(weakTextbox.TryGetTarget(out textbox));

            // act
            BindingManager.Unbind(form);

            ClearReferences(ref vm, ref form, ref textbox);

            GC.Collect(2, GCCollectionMode.Forced, true);
            WriteReferences(vm, form, textbox);

            // assert - all weak references will be null if Unbind cleaned up properly
            Assert.IsFalse(weakForm.TryGetTarget(out form));
            Assert.IsFalse(weakVm.TryGetTarget(out vm));
            Assert.IsFalse(weakTextbox.TryGetTarget(out textbox));
        }

        [TestMethod]
        public void UnbindInEvent_Test_NoBinding()
        {
            var vm = new TestingViewModel();
            var form = new Form();

            var weakVm = new WeakReference<TestingViewModel>(vm);
            var weakForm = new WeakReference<Form>(form);

            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));

            BindingManager.Bind(form).To(vm);

            vm = null;
            form = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
            Debug.WriteLine(vm); // to prevent optimization
            Debug.WriteLine(form); // to prevent optimization

            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));
            
            form.FireEvent("FormClosed", new FormClosedEventArgs(CloseReason.None));

            vm = null;
            form = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
            Debug.WriteLine(vm); // to prevent optimization
            Debug.WriteLine(form); // to prevent optimization

            Assert.IsFalse(weakForm.TryGetTarget(out form));
            Assert.IsFalse(weakVm.TryGetTarget(out vm));
        }

        [TestMethod]
        public void UnbindInEvent_Test_BindingOverUserControls()
        {
            // arrange
            // hard references
            var vm = new TestingViewModel();
            var form = new Form();
            var userControl = new UserControl();

            form.FormClosed += (sender, args) => BindingManager.Unbind((UserControl)((Form)sender).Controls[0]);

            var formLabel = new Label();
            var ucLabel = new Label();
            form.Controls.Add(userControl);
            form.Controls.Add(formLabel);
            userControl.Controls.Add(ucLabel);

            // weak references
            var weakVm = new WeakReference<TestingViewModel>(vm);
            var weakForm = new WeakReference<Form>(form);
            var weakUserControl = new WeakReference<UserControl>(userControl);

            // smole test - weak references are valid
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));

            // binding
            BindingManager.Bind(form).To(vm);
            BindingManager.Bind(userControl).To(vm);
            BindingManager.For(form).Bind(formLabel, _ => _.Text).To(vm, _ => _.Text);
            BindingManager.For(userControl).Bind(ucLabel, _ => _.Text).To(vm, _ => _.Text);

            // clear hard references
            ClearReference(ref form);
            ClearReference(ref vm);
            ClearReference(ref formLabel);
            ClearReference(ref ucLabel);
            ClearReference(ref userControl);

            GC.Collect(2, GCCollectionMode.Forced, true);
            Console.WriteLine(vm); // to prevent optimization
            Console.WriteLine(form); // to prevent optimization
            Console.WriteLine(formLabel); // to prevent optimization
            Console.WriteLine(ucLabel); // to prevent optimization
            Console.WriteLine(userControl); // to prevent optimization

            // assert - weak references are valid, binding manager holds references to controls
            Assert.IsTrue(weakUserControl.TryGetTarget(out userControl));
            Assert.IsTrue(weakForm.TryGetTarget(out form));
            Assert.IsTrue(weakVm.TryGetTarget(out vm));

            // unbind everything
            form.FireEvent("FormClosed", new FormClosedEventArgs(CloseReason.None));

            ClearReference(ref ucLabel);
            ClearReference(ref formLabel);
            ClearReference(ref userControl);
            ClearReference(ref form);
            ClearReference(ref vm);

            // this should clear all weak references, because there are not more hard references
            GC.Collect(2, GCCollectionMode.Forced, true);

            Console.WriteLine(vm); // to prevent optimization
            Console.WriteLine(form); // to prevent optimization
            Console.WriteLine(formLabel); // to prevent optimization
            Console.WriteLine(ucLabel); // to prevent optimization
            Console.WriteLine(userControl); // to prevent optimization

            // assert that the object graph has been collected
            Assert.IsFalse(weakUserControl.TryGetTarget(out userControl));
            Assert.IsFalse(weakForm.TryGetTarget(out form));
            Assert.IsFalse(weakVm.TryGetTarget(out vm));
        }

        [TestMethod]
        public void MultipleViewsSubscriptionTest()
        {
            // arrange
            // setup hard references
            const string InitialText = "Initial";
            const string NewText = "New";

            var vm = new TestingViewModel();
            var form1 = new Form();
            var form2 = new Form();
            var textbox1 = new TextBox();
            var textbox2 = new TextBox();
            form1.Controls.Add(textbox1);
            form2.Controls.Add(textbox2);

            BindingManager.Bind(form1).To(vm);
            BindingManager.Bind(form2).To(vm);
            BindingManager.For(form1).Bind(textbox1, _ => _.Text).To(vm, _ => _.Text);
            BindingManager.For(form2).Bind(textbox2, _ => _.Text).To(vm, _ => _.Text);

            vm.Text = InitialText;
            Assert.AreEqual(InitialText, textbox1.Text);
            Assert.AreEqual(InitialText, textbox2.Text);

            BindingManager.Unbind(form1);

            vm.Text = NewText;
            Assert.AreEqual(InitialText, textbox1.Text);
            Assert.AreEqual(NewText, textbox2.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnbindForm_Null_Throws()
        {
            BindingManager.Unbind((Form)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnbindUserControl_Null_Throws()
        {
            BindingManager.Unbind((UserControl)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindUserControl_Null_Throws()
        {
            BindingManager.Bind((UserControl)null);
        }

        private static void ClearReference<T>(ref T target)
            where T : class
        {
            target = null;
        }

        /// <summary>
        /// This method prevents the compiler from optimizing unused variables away.
        /// </summary>
        private static void WriteReferences<TViewModel, TView, TControl>(TViewModel vm, TView form, TControl label)
        {
            Console.WriteLine(vm); // to prevent optimization
            Console.WriteLine(form); // to prevent optimization
            Console.WriteLine(label); // to prevent optimization
        }

        /// <summary>
        /// Sets the provided references to null.
        /// </summary>
        private static void ClearReferences<TViewModel, TView, TControl>(ref TViewModel vm, ref TView form, ref TControl label)
            where TViewModel : class where TView : class where TControl : class
        {
            vm = null;
            form = null;
            label = null;
        }
    }
}