using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class CollectionsManager
    {
        private static readonly Dictionary<string, MongoCollection> Collections =
            new Dictionary<string, MongoCollection>();

        public static MongoCollection GetCollection(string name)
        {            
            name = GetCollectioName(name);

            if (Collections.ContainsKey(name))
            {
                return Collections[name];
            }

            lock (typeof (CollectionsManager))
            {
                if (!Collections.ContainsKey(name))
                {
                    MongoCollection collection = Helper.Db(name).GetCollection(name);
                    Collections.Add(name, collection);
                }
                return Collections[name];
            }
        }

        //TODO: Pendiente de refactor, meter en un buffer o usarlo siempre tipado.
        public static MongoCollection<T> GetCollection<T>(string name)
        {            
            name = GetCollectioName(name);

            MongoCollection<T> collection = Helper.Db(name).GetCollection<T>(name);
            return collection;
        }

        public static string GetCollectioName(string name)
        {
            if (!name.EndsWith("_Collection"))
            {
                name = String.Format("{0}_Collection", name);
            }
            return name;
        }
    }
}