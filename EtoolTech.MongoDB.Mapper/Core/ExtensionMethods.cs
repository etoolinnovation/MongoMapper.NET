using System.Collections.Generic;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        #region Public Methods

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

        #endregion
    }
}