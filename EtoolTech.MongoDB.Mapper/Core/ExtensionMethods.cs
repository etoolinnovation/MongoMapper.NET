using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        #region Public Methods

        public static void Delete<T>(this T Object) where T : MongoMapper
        {
            ((IMongoMapperWriteable)Object).Delete<T>();
        }

        public static void FillByKey<T>(this T Object, params object[] Values) where T : MongoMapper
        {
            object result = Finder.Instance.FindByKey<T>(Values);

            ReflectionUtility.CopyObject(result, Object);
        }

        public static void FindByMongoId<T>(this T Object, long Id) where T : MongoMapper
        {
            object result = Finder.Instance.FindById<T>(Id);

            ReflectionUtility.CopyObject(result, Object);
        }

        public static void MongoFind<T>(this List<T> List, IMongoQuery Query = null) where T : MongoMapper
        {
            List.Clear();
            List.AddRange(Query == null ? Finder.Instance.AllAsList<T>() : Finder.Instance.FindAsList<T>(Query));
        }

        public static void MongoFind<T>(this List<T> List, string FieldName, object Value) where T : MongoMapper
        {
            List.Clear();
            List.AddRange(Finder.Instance.FindAsList<T>(MongoQuery.Eq(FieldName, Value)));
        }

        public static void MongoFind<T>(this List<T> List, Expression<Func<T, object>> Exp) where T : MongoMapper
        {
            List.Clear();
            List.AddRange(Finder.Instance.FindAsList(Exp));
        }

        public static int Save<T>(this T Object) where T : MongoMapper
        {
            return ((IMongoMapperWriteable)Object).Save<T>();
        }

        public static void ServerUpdate<T>(this T Object, UpdateBuilder Update, bool Refill = true) where T : MongoMapper
        {
            ((IMongoMapperWriteable)Object).ServerUpdate<T>(Update, Refill);
        }

        #endregion
    }
}