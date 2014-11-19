using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class CollectionsManager
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoCollectionName> CustomCollectionsName =
            new Dictionary<string, MongoCollectionName>();

        private static readonly Dictionary<string, MongoCollection> Collections =
            new Dictionary<string, MongoCollection>();

        private static readonly Dictionary<string, MongoCollection> PrimaryCollections =
            new Dictionary<string, MongoCollection>();

        private static readonly Object LockObject = new Object();

        #endregion

        #region Public Methods

        private static string GetCollectioName(string Name)
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

        public static MongoCollection GetCollection(string Name)
        {
            Name = GetCollectioName(Name);

            if (Collections.ContainsKey(Name))
            {
                return Collections[Name];
            }

            lock (LockObject)
            {
                if (!Collections.ContainsKey(Name))
                {
                    MongoCollection collection = MongoMapperHelper.Db(Name).GetCollection(Name);
                    Collections.Add(Name, collection);
                }
                return Collections[Name];
            }
        }

        public static MongoCollection GetPrimaryCollection(string Name)
        {
            Name = GetCollectioName(Name);

            if (PrimaryCollections.ContainsKey(Name))
            {
                return PrimaryCollections[Name];
            }

            lock (LockObject)
            {
                if (!PrimaryCollections.ContainsKey(Name))
                {
                    MongoCollection collection = MongoMapperHelper.Db(Name, true).GetCollection(Name);
                    PrimaryCollections.Add(Name, collection);
                }
                return Collections[Name];
            }
        }

        #endregion
    }
}