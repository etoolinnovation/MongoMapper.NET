using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
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
            var result = CollectionsManager.GetCollection<T>(typeof(T).Name).Find(Builders<T>.Filter.Eq("_id",Id)).Limit(1).ToListAsync().Result;
            if (result.Any())
            {
                return result.First().ToBsonDocument();
            }
            else
            {
                return null;
            }
        }

        public T FindById<T>(long Id)
        {
            var result = CollectionsManager.GetCollection<T>(typeof(T).Name).Find(Builders<T>.Filter.Eq("_id", Id)).Limit(1).ToListAsync().Result;
            if (result.Any())
            {                
                return result.First();
            }
            else
            {
                return default(T);
            }
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

        public long FindIdByKey<T>(Type Type, Dictionary<string, object> KeyValues)
        {
            //Si la key es la interna y vieb
            if (KeyValues.Count == 1 && KeyValues.First().Key == "m_id" && 
                KeyValues.First().Value is long && 
                (long) KeyValues.First().Value == default(long))
            {
                return default(long);
            }

            var query = Builders<T>.Filter.And(KeyValues.Select(KeyValue => Builders<T>.Filter.Eq(KeyValue.Key, KeyValue.Value)).ToArray());

            var result = CollectionsManager.GetCollection<T>(Type.Name).Find(Builders<T>.Filter.And(query)).Project(Builders<T>.Projection.Include("_id")).Limit(1).ToListAsync().Result;

            //if (ConfigManager.Out != null)
            //{
            //    ConfigManager.Out.Write(String.Format("{0}: ", T.Name));
            //    ConfigManager.Out.WriteLine(result.Query.ToString());
            //    ConfigManager.Out.WriteLine(result.Explain().ToJson());
            //    ConfigManager.Out.WriteLine();
            //}

            if (! result.Any())
            {
                return default(long);
            }


            return result.First()["_id"].AsInt64;
        }

        public long FindIdByKey<T>(Dictionary<string, object> KeyValues)
        {            
            return FindIdByKey<T>(typeof(T), KeyValues);                
        }


        public T FindObjectByKey<T>(Dictionary<string, object> KeyValues)
        {
            var query = Builders<T>.Filter.And(KeyValues.Select(KeyValue => Builders<T>.Filter.Eq(KeyValue.Key, KeyValue.Value)).ToArray());

            var result = CollectionsManager.GetCollection<T>(typeof (T).Name).Find(query).Limit(1).ToListAsync().Result;

            if (result == null || !result.Any())
            {
                throw new FindByKeyNotFoundException();
            }
                      
            return result.First();
        }

        #endregion

     
    }
}