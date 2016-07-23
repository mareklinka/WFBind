using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WFbind;

namespace WfBindTests.Bindings
{
    [TestClass]
    public class CheckBoxBindingTests
    {
        [TestMethod]
        public void BindingToView_BasicTest_Form()
        {
            // arrange
            const bool initial = false;
            const bool updated = true;

            var vm = new TestingViewModel { BoolValue = initial };
            var form = new Form();

            var control = new CheckBox();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form).Bind(control, _ => _.Checked).To(vm, _ => _.BoolValue);

            Assert.AreEqual(initial, control.Checked);

            vm.BoolValue = updated;

            // assert
            Assert.AreEqual(updated, control.Checked);
        }

        [TestMethod]
        public void BindingToView_BasicTest_UserControl()
        {
            // arrange
            const bool initial = false;
            const bool updated = true;

            var vm = new TestingViewModel { BoolValue = initial };
            var form = new Form();

            var control = new CheckBox();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form).Bind(control, _ => _.Checked).To(vm, _ => _.BoolValue);

            Assert.AreEqual(initial, control.Checked);

            vm.BoolValue = updated;

            // assert
            Assert.AreEqual(updated, control.Checked);
        }

        [TestMethod]
        public void BindingToViewModel_WithoutTwoWay_NoChange()
        {
            // arrange
            const bool initial = false;
            const bool updated = true;

            var vm = new TestingViewModel { BoolValue = initial };
            var form = new Form();

            var control = new CheckBox();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(control, _ => _.Checked)
                .To(vm, _ => _.BoolValue)
                .Setup(_ => _.IsTwoWay = false);

            Assert.AreEqual(initial, control.Checked);

            control.Checked = updated;

            // assert
            Assert.AreEqual(initial, vm.BoolValue);
        }

        [TestMethod]
        public void BindingToViewModel_WithTwoWay()
        {
            // arrange
            const bool initial = false;
            const bool updated = true;

            var vm = new TestingViewModel { BoolValue = initial };
            var form = new Form();

            var control = new CheckBox();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vm);

            // act
            BindingManager.For(form)
                .Bind(control, _ => _.Checked)
                .To(vm, _ => _.BoolValue)
                .Setup(_ => _.IsTwoWay = true);

            Assert.AreEqual(initial, control.Checked);

            control.Checked = updated;

            // assert
            Assert.AreEqual(updated, vm.BoolValue);
        }

        [TestMethod]
        public void Unbind_Unhooks()
        {
            const bool value = true;

            var vm1 = new TestingViewModel();
            var form = new Form();

            var control = new CheckBox();
            form.Controls.Add(control);

            BindingManager.Bind(form).To(vm1);

            // act
            BindingManager.For(form).Bind(control, _ => _.Checked).To(vm1, _ => _.BoolValue);

            vm1.BoolValue = value;
            Assert.AreEqual(value, control.Checked);

            var vm2 = new TestingViewModel { BoolValue = true };
            BindingManager.Bind(form).To(vm2);

            vm2.BoolValue = !value;
            Assert.AreEqual(value, control.Checked);
        }
    }
}