using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace WFbind
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        /// <summary>
        /// Gets property info from an expression.
        /// </summary>
        /// <typeparam name="TSource">Type of source object.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="source">The source object containing the property.</param>
        /// <param name="propertyLambda">The expression specifying a property on the source object.</param>
        /// <exception cref="ArgumentException">Thrown when a property name cannot be extracted from the provided expression.</exception>
        /// <returns>Property info.</returns>
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
                    propertyLambda));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda,
                    type));

            return propInfo;
        }
    }
}