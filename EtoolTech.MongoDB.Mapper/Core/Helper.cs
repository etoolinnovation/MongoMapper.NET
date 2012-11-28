namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EtoolTech.MongoDB.Mapper.Attributes;
    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Exceptions;

    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public class Helper
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoMapperIdIncrementable> BufferIdIncrementables =
            new Dictionary<string, MongoMapperIdIncrementable>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey =
            new Dictionary<string, List<string>>();

        private static readonly HashSet<string> CustomDiscriminatorTypes = new HashSet<string>();

        private static readonly Object LockObjectCustomDiscritminatorTypes = new Object();

        private static readonly Object LockObjectIdIncrementables = new Object();

        private static readonly Object LockObjectIndex = new Object();

        private static readonly Object LockObjectPk = new Object();

        private static readonly HashSet<Type> SupportedTypesLits = new HashSet<Type>
            { typeof(string), typeof(decimal), typeof(int), typeof(long), typeof(DateTime), typeof(bool) };

        #endregion

        #region Public Methods

        public static MongoDatabase Db(string objName)
        {
            return Db(objName, false);
        }

        public static MongoDatabase Db(string objName, bool primary)
        {
            string databaseName = ConfigManager.DataBaseName(objName);

            string connectionString = ConfigManager.GetConnectionString(objName);

            if (primary) connectionString = string.Format("{0};readPreference=primary", connectionString);

            var client = new MongoClient(connectionString);

            MongoServer server = client.GetServer();

            MongoDatabase dataBase = server.GetDatabase(databaseName);
            return dataBase;
        }

        public static IEnumerable<string> GetPrimaryKey(Type t)
        {
            if (BufferPrimaryKey.ContainsKey(t.Name))
            {
                return BufferPrimaryKey[t.Name];
            }

            lock (LockObjectPk)
            {
                if (!BufferPrimaryKey.ContainsKey(t.Name))
                {
                    var keyAtt = (MongoKey)t.GetCustomAttributes(typeof(MongoKey), false).FirstOrDefault();
                    if (keyAtt != null)
                    {
                        if (String.IsNullOrEmpty(keyAtt.KeyFields))
                        {
                            keyAtt.KeyFields = "MongoMapper_Id";
                        }
                        BufferPrimaryKey.Add(t.Name, keyAtt.KeyFields.Split(',').ToList());
                    }
                    else
                    {
                        BufferPrimaryKey.Add(t.Name, new List<string> { "MongoMapper_Id" });
                    }
                }

                return BufferPrimaryKey[t.Name];
            }
        }

        public static void ValidateType(Type t)
        {
            if (!SupportedTypesLits.Contains(t))
            {
                throw new TypeNotSupportedException(t.Name);
            }
        }
        

        #endregion

        #region Methods

        internal static void RebuildClass(Type classType, bool repairCollection)
        {
            if ((repairCollection || !ConfigManager.Config.Context.Generated)
                && !Db(classType.Name).CollectionExists(CollectionsManager.GetCollectioName(classType.Name)))
            {
                Db(classType.Name).CreateCollection(CollectionsManager.GetCollectioName(classType.Name), null);
            }

            if (!CustomDiscriminatorTypes.Contains(classType.Name))
            {
                lock (LockObjectCustomDiscritminatorTypes)
                {
                    if (!CustomDiscriminatorTypes.Contains(classType.Name))
                    {
                        RegisterCustomDiscriminatorTypes(classType);
                        CustomDiscriminatorTypes.Add(classType.Name);
                    }
                }
            }

            if (!BufferIdIncrementables.ContainsKey(classType.Name))
            {
                lock (LockObjectIdIncrementables)
                {
                    if (!BufferIdIncrementables.ContainsKey(classType.Name))
                    {
                        object m =
                            classType.GetCustomAttributes(typeof(MongoMapperIdIncrementable), false).FirstOrDefault();
                        if (m == null)
                        {
                            BufferIdIncrementables.Add(classType.Name, null);
                        }
                        else
                        {
                            BufferIdIncrementables.Add(classType.Name, (MongoMapperIdIncrementable)m);
                        }
                    }
                }
            }

            //TODO: MongoCollectionName

            if (!ConfigManager.Config.Context.Generated || repairCollection)
            {
                foreach (string index in GetIndexes(classType))
                {
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(classType.Name)).EnsureIndex(
                        index.Split(','));
                }

                string[] pk = GetPrimaryKey(classType).ToArray();
                if (pk.Count(k => k == "MongoMapper_Id") == 0)
                {
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(classType.Name)).EnsureIndex(
                        IndexKeys.Ascending(pk));
                }
            }
        }

        private static IEnumerable<string> GetIndexes(Type t)
        {
            if (BufferIndexes.ContainsKey(t.Name))
            {
                return BufferIndexes[t.Name];
            }

            lock (LockObjectIndex)
            {
                if (!BufferIndexes.ContainsKey(t.Name))
                {
                    BufferIndexes.Add(t.Name, new List<string>());
                    object[] indexAtt = t.GetCustomAttributes(typeof(MongoIndex), false);

                    foreach (object index in indexAtt)
                    {
                        if (index != null)
                        {
                            BufferIndexes[t.Name].Add(((MongoIndex)index).IndexFields);
                        }
                    }
                }

                return BufferIndexes[t.Name];
            }
        }

        private static void RegisterCustomDiscriminatorTypes(Type classType)
        {
            object[] regTypes = classType.GetCustomAttributes(typeof(MongoCustomDiscriminatorType), false);

            foreach (object regType in regTypes)
            {
                if (regType != null)
                {
                    var mongoCustomDiscriminatorType = (MongoCustomDiscriminatorType)regType;
                    BsonSerializer.RegisterDiscriminator(
                        mongoCustomDiscriminatorType.Type, mongoCustomDiscriminatorType.Type.Name);
                }
            }
        }

        #endregion
    }
}