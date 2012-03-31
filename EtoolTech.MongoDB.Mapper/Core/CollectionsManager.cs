using System;
using System.Collections.Generic;
using MongoDB.Driver;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper
{
    public class CollectionsManager
    {
        private static readonly Dictionary<string, MongoCollection> Collections = new Dictionary<string, MongoCollection>();

        internal static readonly Dictionary<string, MongoCollectionName> CustomCollectionsName = new Dictionary<string, MongoCollectionName>();

        private static readonly Object LockObject = new Object();

        public static MongoCollection GetCollection(string name)
        {            
            name = GetCollectioName(name);

            if (Collections.ContainsKey(name))
            {
                return Collections[name];
            }

            lock (LockObject)
            {
                if (!Collections.ContainsKey(name))
                {
                    MongoCollection collection = Helper.Db(name).GetCollection(name);
                    Collections.Add(name, collection);
                }
                return Collections[name];
            }
        }
     
        public static string GetCollectioName(string name)
        {
            if (!name.EndsWith("_Collection"))
            {
                name = String.Format("{0}_Collection", name);
            }

            if (CustomCollectionsName.ContainsKey(name)) return CustomCollectionsName[name].Name;
            
            return name;
        }
    }
}