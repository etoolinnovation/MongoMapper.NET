using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        public static void FindByKey<T>(this T o, params object[] values)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }
            
            object result = Finder.Instance.FindByKey<T>(values);

            ReflectionUtility.CopyObject(result,o);
        }

        public static void MongoFind<T>(this List<T> list, QueryComplete query = null)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(query == null ? Finder.Instance.AllAsList<T>() : Finder.Instance.FindAsList<T>(query));
        }

        public static void MongoFind<T>(this List<T> list, string fieldName, object value)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList<T>(MongoQuery.Eq(fieldName, value)));
        }

        public static void MongoFind<T>(this List<T> list, Expression<Func<T, object>> exp)
        {
            if (typeof (T).BaseType != typeof (MongoMapper))
            {
                throw new NotSupportedException();
            }
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList(exp));
        }
    }

}

