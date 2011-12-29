using MongoDB.Bson;

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
        private static readonly List<Type> SupportedTypesLits = new List<Type>
            { typeof(string), typeof(decimal), typeof(int), typeof(long), typeof(DateTime), typeof(bool) };

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey =
            new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly List<string> CustomDiscriminatorTypes = new List<string>();

        private static MongoDatabase _dataBase;

        private static MongoServer _server;

        public static MongoDatabase Db
        {
            get
            {
                if (_server == null)
                {

                    //TODO: Revisar donde ponerlo, posibilidad de definirlo por coleccion??
                    BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize * 1024 * 1024; 
                    
                    MongoServerSettings ServerSettings = new MongoServerSettings();
                    string userName = ConfigManager.UserName;

                    if (!String.IsNullOrEmpty(userName))
                    {
                        ServerSettings.DefaultCredentials = new MongoCredentials(userName, ConfigManager.PassWord);
                    }

                    ServerSettings.Server = new MongoServerAddress(ConfigManager.Host, ConfigManager.Port);
                    ServerSettings.MaxConnectionPoolSize = ConfigManager.PoolSize;
                    //TODO: Connection Mode a la config
                    ServerSettings.ConnectionMode = ConnectionMode.Direct;
                    ServerSettings.WaitQueueTimeout = TimeSpan.FromSeconds(ConfigManager.WaitQueueTimeout);

                    _server = MongoServer.Create(ServerSettings);
                }

                return _dataBase ?? (_dataBase = _server.GetDatabase(ConfigManager.DataBaseName));
            }
        }

        public static void ValidateType(Type t)
        {
            if (!SupportedTypesLits.Contains(t))
            {
                throw new TypeNotSupportedException(t.Name);
            }
        }

        public static IEnumerable<string> GetPrimaryKey(Type t)
        {
            if (BufferPrimaryKey.ContainsKey(t.Name))
            {
                return BufferPrimaryKey[t.Name];
            }

            lock (typeof(Helper))
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
                        BufferPrimaryKey.Add(t.Name, new List<string>() { "MongoMapper_Id" });
                    }
                }

                return BufferPrimaryKey[t.Name];
            }
        }

        private static IEnumerable<string> GetIndexes(Type t)
        {
            if (BufferIndexes.ContainsKey(t.Name))
            {
                return BufferIndexes[t.Name];
            }

            lock (typeof(Helper))
            {
                if (!BufferIndexes.ContainsKey(t.Name))
                {
                    BufferIndexes.Add(t.Name, new List<string>());
                    var indexAtt = t.GetCustomAttributes(typeof(MongoIndex), false);

                    foreach (var index in indexAtt)
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

        internal static void RebuildClass(Type classType, bool repairCollection)
        {
            if (repairCollection && !ConfigManager.Config.Context.Generated
                && !Db.CollectionExists(CollectionsManager.GetCollectioName(classType.Name)))
            {
                Db.CreateCollection(CollectionsManager.GetCollectioName(classType.Name), null);
            }

            if (!CustomDiscriminatorTypes.Contains(classType.Name))
            {
                RegisterCustomDiscriminatorTypes(classType);
                CustomDiscriminatorTypes.Add(classType.Name);
            }

            if (!ConfigManager.Config.Context.Generated || repairCollection)
            {
                foreach (string index in GetIndexes(classType))
                {
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(classType.Name)).EnsureIndex(
                        index.Split(','));
                }
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(classType.Name)).EnsureIndex(
                    IndexKeys.Ascending(GetPrimaryKey(classType).ToArray()));
            }
        }

        private static void RegisterCustomDiscriminatorTypes(Type classType)
        {
            var RegTypes = classType.GetCustomAttributes(typeof(MongoCustomDiscriminatorType), false);

            foreach (var RegType in RegTypes)
            {
                if (RegType != null)
                {
                    MongoCustomDiscriminatorType MongoCustomDiscriminatorType = (MongoCustomDiscriminatorType)RegType;
                    BsonDefaultSerializer.RegisterDiscriminator(
                        MongoCustomDiscriminatorType.Type, MongoCustomDiscriminatorType.Type.Name);
                }
            }
        }
    }
}