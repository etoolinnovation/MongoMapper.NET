namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using EtoolTech.MongoDB.Mapper.Interfaces;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public static class ExtensionMethods
    {
        #region Public Methods

        public static void Delete<T>(this T o) where T : MongoMapper
        {         
            ((IMongoMapperWriteable)o).Delete<T>();
        }

        public static void FillByKey<T>(this T o, params object[] values) where T : MongoMapper
        {          
            object result = Finder.Instance.FindByKey<T>(values);

            ReflectionUtility.CopyObject(result, o);
        }

        public static void FindByMongoId<T>(this T o, long Id) where T : MongoMapper
        {
    
            object result = Finder.Instance.FindById<T>(Id);

            ReflectionUtility.CopyObject(result, o);
        }

        public static void MongoFind<T>(this List<T> list, IMongoQuery query = null) where T : MongoMapper
        {
            list.Clear();
            list.AddRange(query == null ? Finder.Instance.AllAsList<T>() : Finder.Instance.FindAsList<T>(query));
        }

        public static void MongoFind<T>(this List<T> list, string fieldName, object value) where T : MongoMapper
        {
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList<T>(MongoQuery.Eq(fieldName, value)));
        }

        public static void MongoFind<T>(this List<T> list, Expression<Func<T, object>> exp) where T : MongoMapper
        {
            list.Clear();
            list.AddRange(Finder.Instance.FindAsList(exp));
        }

        public static int Save<T>(this T o) where T : MongoMapper
        {   
            return ((IMongoMapperWriteable)o).Save<T>();
        }

        public static void ServerUpdate<T>(this T o, UpdateBuilder update, bool refill = true) where T : MongoMapper
        {         
            ((IMongoMapperWriteable)o).ServerUpdate<T>(update, refill);
        }

        #endregion
    }
}