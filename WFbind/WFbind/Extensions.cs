using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace WFbind
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                // this is required to handle value types, whose expressions will contain a cast
                var unary = propertyLambda.Body as UnaryExpression;

                if (unary == null || unary.NodeType != ExpressionType.Convert)
                {
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a method, not a property.",
                        propertyLambda));
                }

                member = unary.Operand as MemberExpression;

                if (member == null)
                {
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a method, not a property.",
                        propertyLambda));
                }
            }

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

        public static ViewModelConfiguration<Form> Bind(this Form form)
        {
            return BindingManager.Bind(form);
        }

        public static ViewModelConfiguration<UserControl> Bind(this UserControl form)
        {
            return BindingManager.Bind(form);
        }

        public static INotifyPropertyChanged DataContext(this UserControl form)
        {
            return BindingManager.GetViewModelFor<INotifyPropertyChanged>(form);
        }

        public static INotifyPropertyChanged DataContext(this Form form)
        {
            return BindingManager.GetViewModelFor<INotifyPropertyChanged>(form);
        }

        public static TViewModel DataContext<TViewModel>(this UserControl form) where TViewModel : INotifyPropertyChanged
        {
            return BindingManager.GetViewModelFor<TViewModel>(form);
        }

        public static TViewModel DataContext<TViewModel>(this Form form) where TViewModel : INotifyPropertyChanged
        {
            return BindingManager.GetViewModelFor<TViewModel>(form);
        }
    }
}