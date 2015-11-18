using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        #region Public Methods

        public static void FillByKey<T>(this T Object, params object[] Values) where T : MongoMapper<T>
        {
            object result = Finder.Instance.FindByKey<T>(Values);

            ReflectionUtility.CopyObject(result, Object);
        }

        public static void FindByMongoId<T>(this T Object, long Id) where T : MongoMapper<T>
        {
            object result = Finder.Instance.FindById<T>(Id);

            ReflectionUtility.CopyObject(result, Object);
        }

        public static void MongoFind<T>(this List<T> List) where T : MongoMapper<T>
        {
            List.Clear();
            var col = new MongoMapperCollection<T>();
            col.Find(new BsonDocument());
            List.AddRange(col.ToList());
        }

        public static void MongoFind<T>(this List<T> List, FilterDefinition<T> Query) where T : MongoMapper<T>
        {
            List.Clear();
            var col = new MongoMapperCollection<T>();
            col.Find(Query);
            List.AddRange(col.ToList());
        }

        public static void MongoFind<T>(this List<T> List, Expression<Func<T, object>> Field, object Value) where T : MongoMapper<T>
        {
            List.Clear();
            var col = new MongoMapperCollection<T>();
            col.Find(MongoQuery<T>.Eq(Field, Value));
            List.AddRange(col.ToList());
        }

        public static void MongoFind<T>(this List<T> List, string FieldName, object Value) where T : MongoMapper<T>
        {
            List.Clear();
            var col = new MongoMapperCollection<T>();
            col.Find(MongoQuery<T>.Eq(typeof(T).Name, FieldName, Value));
            List.AddRange(col.ToList());
        }

        #endregion
    }
}