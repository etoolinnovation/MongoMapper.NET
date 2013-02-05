using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ReflectionUtility
    {
        #region Public Methods

        public static void BuildSchema(Assembly Assembly)
        {
            List<Type> types = Assembly.GetTypes().Where(t => t.BaseType == typeof (MongoMapper)).ToList();
            foreach (Type type in types)
            {
                Helper.RebuildClass(type, true);
            }
        }

        public static void CopyObject<T>(T Source, T Destination)
        {
            Type t = Source.GetType();

            foreach (PropertyInfo property in t.GetProperties())
            {
                property.SetValue(Destination, property.GetValue(Source, null), null);
            }
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> Exp)
        {
            var memberExpression = Exp.Body as UnaryExpression;
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
                var memberexpresion2 = Exp.Body as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }

            return string.Empty;
        }

        public static object GetPropertyValue(object Obj, string PropertyName)
        {
            if ((PropertyName == "m_id") && (Obj is IMongoMapperIdeable))
            {
                return ((IMongoMapperIdeable) Obj).m_id;
            }

            Type t = Obj.GetType();
            PropertyInfo property = t.GetProperty(PropertyName);
            return property.GetValue(Obj, null);
        }

        public static T GetPropertyValue<T>(object Obj, string PropertyName)
        {
            return (T) GetPropertyValue(Obj, PropertyName);
        }

        #endregion
    }
}