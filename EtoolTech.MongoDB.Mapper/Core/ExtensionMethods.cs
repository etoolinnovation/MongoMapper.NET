using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using EtoolTech.MongoDB.Mapper.Interfaces;

using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ExtensionMethods
    {
        private static readonly IFinder Finder = new Finder();

        public static void MongoFind<T>(this List<T> list, QueryComplete query = null)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotImplementedException();
            }
            list.Clear();
            list.AddRange(query == null ? Finder.AllAsList<T>() : Finder.FindAsList<T>(query));
        }

        public static void MongoFind<T>(this List<T> list, string fieldName, object value)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotImplementedException();
            }
            list.Clear();
            list.AddRange(Finder.FindAsList<T>(MongoQuery.Eq(fieldName, value)));
        }

        public static void MongoFind<T>(this List<T> list, Expression<Func<T, object>> exp)
        {
            if (typeof(T).BaseType != typeof(MongoMapper))
            {
                throw new NotImplementedException();
            }
            list.Clear();
            list.AddRange(Finder.FindAsList<T>(exp));
        }
    }
}