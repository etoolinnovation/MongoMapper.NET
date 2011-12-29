using System;
using System.Linq.Expressions;

namespace EtoolTech.MongoDB.Mapper
{
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
    }
}