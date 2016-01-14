using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class CollectionsManager
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoCollectionName> CustomCollectionsName =
            new Dictionary<string, MongoCollectionName>();

        private static readonly Dictionary<string, object> Collections = new Dictionary<string, object>();

        private static readonly Dictionary<string, object> PrimaryCollections = new Dictionary<string, object>();

        private static readonly Object LockObject = new Object();

        #endregion

        #region Public Methods

        internal static string GetCollectioName(string Name)
        {
            if (CustomCollectionsName.ContainsKey(Name))
            {
                return CustomCollectionsName[Name].Name;
            }
            
            if (!Name.EndsWith("_Collection"))
            {
                Name = String.Format("{0}_Collection", Name);
            }         

            return Name;
        }

        public static IEnumerable<string> GetCollentionNames(string DbName)
        {
            var colNames = new List<string>();
            MongoMapperHelper.Db(DbName).ListCollectionsAsync().Result.ForEachAsync(F => colNames.Add(F["name"].AsString));
            return colNames.Where(C => !C.ToUpper().StartsWith("SYSTEM"));
        }

        public static IMongoCollection<T> GetCollection<T>(string Name)
        {
            Name = GetCollectioName(Name);

            string key = Name + "|" + typeof (T).FullName;

            if (Collections.ContainsKey(key))
            {
                return (IMongoCollection<T>) Collections[key];
            }

            lock (LockObject)
            {
                if (!Collections.ContainsKey(key))
                {
                    var collection = MongoMapperHelper.Db(Name).GetCollection<T>(Name);
                    Collections.Add(key, collection);
                }
                return (IMongoCollection<T>) Collections[key];
            }
        }

        public static IMongoCollection<T> GetPrimaryCollection<T>(string Name)
        {
            Name = GetCollectioName(Name);

            string key = Name + "|" + typeof(T).FullName;

            if (PrimaryCollections.ContainsKey(key))
            {
                return (IMongoCollection<T>) PrimaryCollections[key];
            }

            lock (LockObject)
            {
                if (!PrimaryCollections.ContainsKey(key))
                {
                    var collection = MongoMapperHelper.Db(Name, true).GetCollection<T>(Name);
                    PrimaryCollections.Add(key, collection);
                }
                return (IMongoCollection<T>) Collections[key];
            }
        }


        public static bool CollectionExists(string Name)
        {
            var filter = new BsonDocument("name", Name);
            //filter by collection name
            var collections = MongoMapperHelper.Db(Name).ListCollectionsAsync(new ListCollectionsOptions { Filter = filter }).GetAwaiter().GetResult().ToListAsync().Result;
            //check for existence
            return collections.Any();
        }

        #endregion
    }
}