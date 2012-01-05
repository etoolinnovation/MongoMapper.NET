using System;
using System.Linq.Expressions;

namespace EtoolTech.MongoDB.Mapper
{
    using System.Reflection;

    public static class ReflectionUtility
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> exp)
        {
            var memberExpression = exp.Body as UnaryExpression;
            if (memberExpression != null)
            {
                var memberexpresion2 = memberExpression.Operand as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }
            else
            {
                var memberexpresion2 = exp.Body as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }

            return string.Empty;
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo property = t.GetProperty(propertyName);
            var objParm = Expression.Parameter(obj.GetType(), "o");
            Type delegateType = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var lambda = Expression.Lambda(delegateType, Expression.Property(objParm, property.Name), objParm);
            return lambda.Compile().DynamicInvoke(obj);
        }

         public static T GetPropertyValue<T>(object obj, string propertyName)
         {
             return (T)GetPropertyValue(obj, propertyName);
         }
     
    }
}