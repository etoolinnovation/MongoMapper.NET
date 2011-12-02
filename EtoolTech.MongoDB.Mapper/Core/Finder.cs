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

namespace EtoolTech.MongoDB.Mapper.Core
{
    //TODO: Pendiente implementar pedir solo un subset de campos
    public class Finder : IFinder
    {
        #region FindAsList Methods

        public T FindById<T>(Guid id)
        {
            QueryComplete query = Query.EQ("_id", id);
            return Helper.GetCollection(typeof (T).Name).FindOneAs<T>(query);
        }

        public BsonDocument FindBsonDocumentById<T>(Guid id)
        {
            QueryComplete query = Query.EQ("_id", id);
            return Helper.GetCollection(typeof (T).Name).FindOneAs<T>(query).ToBsonDocument();
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
                if (keyValue.Value is int)
                    queryList.Add(GetEqQuery(typeof (int), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is string)
                    queryList.Add(GetEqQuery(typeof (string), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is DateTime)
                    queryList.Add(GetEqQuery(typeof (DateTime), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is long)
                    queryList.Add(GetEqQuery(typeof (long), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is bool)
                    queryList.Add(GetEqQuery(typeof (bool), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is double)
                    queryList.Add(GetEqQuery(typeof (double), keyValue.Key, keyValue.Value));
                else if (keyValue.Value is Guid)
                    queryList.Add(GetEqQuery(typeof (Guid), keyValue.Key, keyValue.Value));
                else
                {
                    throw new TypeNotSupportedException(keyValue.Value.GetType().Name);
                }
            }

            QueryComplete query = Query.And(queryList.ToArray());

            MongoCursor<T> result = Helper.GetCollection(typeof (T).Name).FindAs<T>(query);            
            if (result.Count() == 0)
                throw new FindByKeyNotFoundException();
            return result.First();
        }

        public Guid FindGuidByKey<T>(Dictionary<string, object> keyValues)
        {
            var queryList = new List<QueryComplete>();
            foreach (var keyValue in keyValues)
            {
                if (keyValue.Value is int)
                    queryList.Add(GetEqQuery(typeof (int), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is string)
                    queryList.Add(GetEqQuery(typeof (string), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is DateTime)
                    queryList.Add(GetEqQuery(typeof (DateTime), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is long)
                    queryList.Add(GetEqQuery(typeof (long), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is bool)
                    queryList.Add(GetEqQuery(typeof (bool), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is double)
                    queryList.Add(GetEqQuery(typeof (double), keyValue.Key, keyValue.Value));

                else if (keyValue.Value is Guid)
                    queryList.Add(GetEqQuery(typeof (Guid), keyValue.Key, keyValue.Value));
                else
                {
                    throw new TypeNotSupportedException(keyValue.Value.GetType().Name);
                }
            }

            QueryComplete query = Query.And(queryList.ToArray());

            MongoCursor<T> result =
                Helper.GetCollection(typeof (T).Name).FindAs<T>(query).SetFields(Fields.Include("_id"));
            if (result.Count() == 0)
                return Guid.Empty;

            object oId;
            Type oType;
            IIdGenerator iGen;
            result.First().ToBsonDocument().GetDocumentId(out oId, out oType, out iGen);
            return (Guid) oId;
        }


        public List<T> FindAsList<T>(QueryComplete query = null)
        {
            return FindAsCursor<T>(query).ToList();
        }

        public MongoCursor<T> FindAsCursor<T>(QueryComplete query = null)
        {
            if (query == null)
                return Helper.GetCollection(typeof (T).Name).FindAllAs<T>();

            return Helper.GetCollection(typeof (T).Name).FindAs<T>(query);
        }
         

        public List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {
            return FindAsCursor<T>(exp).ToList();
        }

        //TODO: Pendiente de probar
        public MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            var ep = new ExpressionParser();
            QueryComplete query = ep.ParseExpression(exp);
            return Helper.GetCollection(typeof (T).Name).FindAs<T>(query);
        }

        #endregion

        #region GetQuerys

   

        public QueryComplete GetEqQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (value is string)
            {
                bool IsLike = false;
                string txtValue = value.ToString();
                if (txtValue.StartsWith("%") && txtValue.EndsWith("%"))
                {
                    value = String.Format("/{0}/", txtValue);
                    IsLike = true;
                }
                else if (txtValue.StartsWith("%"))
                {
                    value = String.Format("/{0}^/", txtValue);
                    IsLike = true;
                }
                else if (txtValue.EndsWith("%"))
                {
                    value = String.Format("/^{0}/", txtValue);
                    IsLike = true;
                }

                if (IsLike)
                {
                    query = GetRegEx(fieldName, value.ToString());
                    return query;
                }
            }

            if (type == typeof (DateTime))
                query = Query.EQ(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.EQ(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.EQ(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.EQ(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.EQ(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.EQ(fieldName, (double) value);
            else if (type == typeof (Guid))
                query = Query.EQ(fieldName, (Guid) value);


            return query;
        }

        public QueryComplete GetGtQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (type == typeof (DateTime))
                query = Query.GT(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.GT(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.GT(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.GT(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.GT(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.GT(fieldName, (double) value);

            return query;
        }

        public QueryComplete GetGteQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (type == typeof (DateTime))
                query = Query.GTE(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.GTE(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.GTE(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.GTE(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.GTE(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.GTE(fieldName, (double) value);

            return query;
        }

        public QueryComplete GetLtQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (type == typeof (DateTime))
                query = Query.LT(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.LT(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.LT(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.LT(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.LT(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.LT(fieldName, (double) value);

            return query;
        }

        public QueryComplete GetLteQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (type == typeof (DateTime))
                query = Query.LTE(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.LTE(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.LTE(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.LTE(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.LTE(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.LTE(fieldName, (double) value);

            return query;
        }

        public QueryComplete GetNeQuery(Type type, string fieldName, object value)
        {
            QueryComplete query = null;

            if (type == typeof (DateTime))
                query = Query.NE(fieldName, (DateTime) value);
            else if (type == typeof (int))
                query = Query.NE(fieldName, (int) value);
            else if (type == typeof (string))
                query = Query.NE(fieldName, (string) value);
            else if (type == typeof (long))
                query = Query.NE(fieldName, (long) value);
            else if (type == typeof (bool))
                query = Query.NE(fieldName, (bool) value);
            else if (type == typeof (double))
                query = Query.NE(fieldName, (double) value);

            return query;
        }

        public QueryComplete GetRegEx(string fieldName, string expresion)
        {
            return Query.Matches(fieldName, expresion);
        }

        #endregion

        #region IFinder Members

        public List<T> AllAsList<T>()
        {
            return AllAsCursor<T>().ToList();
        }

        public MongoCursor<T> AllAsCursor<T>()
        {
            return Helper.GetCollection(typeof (T).Name).FindAllAs<T>();
        }

        #endregion
    }
}