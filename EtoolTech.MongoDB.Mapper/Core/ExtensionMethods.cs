using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {

        public static void Save<T>(this T o)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }            

            ((IMongoMapperWriteable)o).Save<T>();

        }

        public static void Delete<T>(this T o)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }

            ((IMongoMapperWriteable)o).Delete<T>();

        }

        public static void FillByKey<T>(this T o, params object[] values)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }
            
            object result = Finder.Instance.FindByKey<T>(values);

            ReflectionUtility.CopyObject(result,o);
        }

        public static void FindByMongoId<T>(this T o, long Id)
        {
            if (typeof(T) != typeof(MongoMapper) && typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotSupportedException();
            }

            object result = Finder.Instance.FindById<T>(Id);

            ReflectionUtility.CopyObject(result, o);
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

