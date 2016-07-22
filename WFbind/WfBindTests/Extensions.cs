using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WFbind.Tests
{
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