using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WFbind.Tests
{
    [TestClass]
    public class TextBoxBindingTests
    {
        [TestMethod]
        public void BindingToView_BasicTest()
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
    }

    public static class Extensions
    {
        internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        public static void FireEvent<TSource>(this TSource targetObject, string eventName, EventArgs e)
        {
            /*
    * By convention event handlers are internally called by a protected
    * method called OnEventName
    * e.g.
    *     public event TextChanged
    * is triggered by
    *     protected void OnTextChanged
    * 
    * If the object didn't create an OnXxxx protected method,
    * then you're screwed. But your alternative was over override
    * the method and call it - so you'd be screwed the other way too.
    */
            var methodName = "On" + eventName;

            var mi = targetObject.GetType().GetMethod(
                  methodName,
                  BindingFlags.Instance | BindingFlags.NonPublic);

            if (mi == null)
            {
                throw new ArgumentException("Cannot find event thrower named " + methodName);
            }

            mi.Invoke(targetObject, new object[] { e });
        }
    }
}