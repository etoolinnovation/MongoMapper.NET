namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;

    using EtoolTech.MongoDB.Mapper.Attributes;

    using global::MongoDB.Driver;

    public class CollectionsManager
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoCollectionName> CustomCollectionsName =
            new Dictionary<string, MongoCollectionName>();

        private static readonly Dictionary<string, MongoCollection> Collections = new Dictionary<string, MongoCollection>();

        private static readonly Dictionary<string, MongoCollection> PrimaryCollections = new Dictionary<string, MongoCollection>();

        private static readonly Object LockObject = new Object();

        #endregion

        #region Public Methods

        public static string GetCollectioName(string name)
        {
            if (!name.EndsWith("_Collection"))
            {
                name = String.Format("{0}_Collection", name);
            }

            if (CustomCollectionsName.ContainsKey(name))
            {
                return CustomCollectionsName[name].Name;
            }

            return name;
        }

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

        public static MongoCollection GetPrimaryCollection(string name)
        {
            name = GetCollectioName(name);

            if (PrimaryCollections.ContainsKey(name))
            {
                return PrimaryCollections[name];
            }

            lock (LockObject)
            {
                if (!PrimaryCollections.ContainsKey(name))
                {
                    MongoCollection collection = Helper.Db(name, true).GetCollection(name);
                    PrimaryCollections.Add(name, collection);
                }
                return Collections[name];
            }
        }

        #endregion
    }
}