using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Finder : IFinder
    {
        internal static IFinder Instance {get {return new Finder();}}

        private Finder() {}

        #region FindAsList Methods

        public T FindById<T>(long id)
        {
            QueryComplete query = Query.EQ("_id", id);
            return CollectionsManager.GetCollection(typeof (T).Name).FindOneAs<T>(query);
        }

        public BsonDocument FindBsonDocumentById<T>(long id)
        {
            QueryComplete query = Query.EQ("_id", id);
            return CollectionsManager.GetCollection(typeof (T).Name).FindOneAs<T>(query).ToBsonDocument();
        }

        public T FindByKey<T>(params object[] values)
        {
            List<string> fields = Helper.GetPrimaryKey(typeof (T)).ToList();
            var keyValues = new Dictionary<string, object>();
            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i].ToUpper() == "MongoMapper_id".ToUpper() ? "_id" : fields[i];
                keyValues.Add(field, values[i]);
            }

            return FindObjectByKey<T>(keyValues);
        }

        public T FindObjectByKey<T>(Dictionary<string, object> keyValues)
        {
            var queryList = new List<QueryComplete>();
            foreach (var keyValue in keyValues)
            {
                queryList.Add(MongoQuery.Eq(keyValue.Key, keyValue.Value));
            }

            QueryComplete query = Query.And(queryList.ToArray());

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);
            if (!result.Any())
            {
                throw new FindByKeyNotFoundException();
            }
            return result.First();
        }

        public long FindIdByKey<T>(Dictionary<string, object> keyValues)
        {
            //Si la key es la interna y vieb
            if (keyValues.Count == 1 && keyValues.First().Key == "MongoMapper_Id" && keyValues.First().Value is long
                && (long) keyValues.First().Value == default(long))
            {
                return default(long);
            }

            QueryComplete query = Query.And(keyValues.Select(keyValue => MongoQuery.Eq(keyValue.Key, keyValue.Value)).ToArray());

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query).SetFields(Fields.Include("_id"));
            if (!result.Any())
            {
                return default(long);
            }

            object oId;
            Type oType;
            IIdGenerator iGen;
            result.First().ToBsonDocument().GetDocumentId(out oId, out oType, out iGen);
            return (long) oId;
        }

        public List<T> FindAsList<T>(QueryComplete query = null)
        {
            return FindAsCursor<T>(query).ToList();
        }

        public MongoCursor<T> FindAsCursor<T>(QueryComplete query = null)
        {
            if (query == null)
            {
                return CollectionsManager.GetCollection(typeof (T).Name).FindAllAs<T>();
            }

            return CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);
        }

        public List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {
            return FindAsCursor(exp).ToList();
        }

        //TODO: Pendiente de probar
        public MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            var ep = new ExpressionParser();
            QueryComplete query = ep.ParseExpression(exp);
            return CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);
        }

        #endregion

        #region IFinder Members

        public List<T> AllAsList<T>()
        {
            return AllAsCursor<T>().ToList();
        }

        public MongoCursor<T> AllAsCursor<T>()
        {
            return CollectionsManager.GetCollection(typeof (T).Name).FindAllAs<T>();
        }

        #endregion
    }
}