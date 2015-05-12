using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Finder : IFinder
    {
        #region Constructors and Destructors

        private Finder()
        {
        }

        #endregion

        #region Properties

        internal static IFinder Instance
        {
            get { return new Finder(); }
        }

        #endregion

        #region Public Methods

      

        public BsonDocument FindBsonDocumentById<T>(long Id)
        {
            var result = CollectionsManager.GetCollection(typeof (T).Name).FindOneByIdAs<T>(Id);
            return result.ToBsonDocument();
        }

        public T FindById<T>(long Id)
        {
            var result = CollectionsManager.GetCollection(typeof (T).Name).FindOneByIdAs<T>(Id);
            return result;
        }

        public object FindById(Type Type, long Id)
        {
            object result = CollectionsManager.GetCollection(Type.Name).FindOneByIdAs(Type, Id);
            return result;
        }

        public T FindByKey<T>(params object[] Values)
        {
            List<string> fields = MongoMapperHelper.GetPrimaryKey(typeof (T)).ToList();
            var keyValues = new Dictionary<string, object>();
            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i].ToUpper() == "m_id".ToUpper() ? "_id" : fields[i];
                keyValues.Add(field, Values[i]);
            }

            return FindObjectByKey<T>(keyValues);
        }

        public long FindIdByKey(Type T, Dictionary<string, object> KeyValues)
        {
            //Si la key es la interna y vieb
            if (KeyValues.Count == 1 && KeyValues.First().Key == "m_id" && KeyValues.First().Value is long
                && (long) KeyValues.First().Value == default(long))
            {
                return default(long);
            }

            IMongoQuery query =
                Query.And(KeyValues.Select(KeyValue => MongoQuery.Eq(T.Name, KeyValue.Key, KeyValue.Value)).ToArray());

            MongoCursor result =
                CollectionsManager.GetCollection(T.Name).FindAs(T, query).SetFields(Fields.Include("_id"));

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", T.Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                return default(long);
            }


            return ((IMongoMapperIdeable) result.Cast<object>().First()).m_id;
        }

        public long FindIdByKey<T>(Dictionary<string, object> KeyValues)
        {
            //Si la key es la interna y vieb
            if (KeyValues.Count == 1 && KeyValues.First().Key == "m_id" && KeyValues.First().Value is long
                && (long) KeyValues.First().Value == default(long))
            {
                return default(long);
            }

            IMongoQuery query =
                Query.And(KeyValues.Select(KeyValue => MongoQuery.Eq(typeof(T).Name,KeyValue.Key, KeyValue.Value)).ToArray());

            MongoCursor<T> result =
                CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query).SetFields(Fields.Include("_id"));

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof (T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                return default(long);
            }

            return ((IMongoMapperIdeable) result.First()).m_id;
        }


        public T FindObjectByKey<T>(Dictionary<string, object> KeyValues)
        {
            IMongoQuery query =
                Query.And(KeyValues.Select(KeyValue => MongoQuery.Eq(typeof(T).Name, KeyValue.Key, KeyValue.Value)).ToArray());

            var result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);

            if (result == null || result.Size() == 0)
            {
                throw new FindByKeyNotFoundException();
            }
                      
            return result.First();
        }

        #endregion

     
    }
}