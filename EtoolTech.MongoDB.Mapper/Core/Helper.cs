using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Helper
    {
        private static readonly HashSet<Type> SupportedTypesLits = new HashSet<Type>
                                                                       {
                                                                           typeof (string),
                                                                           typeof (decimal),
                                                                           typeof (int),
                                                                           typeof (long),
                                                                           typeof (DateTime),
                                                                           typeof (bool)
                                                                       };

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey =
            new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly HashSet<string> CustomDiscriminatorTypes = new HashSet<string>();

        private static readonly Object _lockObjectPk = new Object();

        private static readonly Object _lockObjectIndex = new Object();


        public static MongoDatabase Db(string objName)
        {
            //TODO: Revisar donde ponerlo?????
            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(objName)*1024*1024;

            string LoginString = "";
            string userName = ConfigManager.UserName(objName);

            if (!String.IsNullOrEmpty(userName))
            {
                LoginString = String.Format("{0}:{1}@", userName, ConfigManager.PassWord(objName));
            }

            string DatabaseName = ConfigManager.DataBaseName(objName);

            string connectionString = String.Format("mongodb://{4}{0}:{1}/{5}?maxpoolsize={2};waitQueueTimeout={3}ms",
                                                    ConfigManager.Host(objName), ConfigManager.Port(objName),
                                                    ConfigManager.PoolSize(objName),
                                                    ConfigManager.WaitQueueTimeout(objName)*1000, LoginString,DatabaseName);

            MongoServer _server = MongoServer.Create(connectionString);

            MongoDatabase _dataBase = _server.GetDatabase(DatabaseName);
            return _dataBase;
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

            lock (_lockObjectPk)
            {
                if (!BufferPrimaryKey.ContainsKey(t.Name))
                {
                    var keyAtt = (MongoKey) t.GetCustomAttributes(typeof (MongoKey), false).FirstOrDefault();
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
                        BufferPrimaryKey.Add(t.Name, new List<string> {"MongoMapper_Id"});
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

            lock (_lockObjectIndex)
            {
                if (!BufferIndexes.ContainsKey(t.Name))
                {
                    BufferIndexes.Add(t.Name, new List<string>());
                    object[] indexAtt = t.GetCustomAttributes(typeof (MongoIndex), false);

                    foreach (object index in indexAtt)
                    {
                        if (index != null)
                        {
                            BufferIndexes[t.Name].Add(((MongoIndex) index).IndexFields);
                        }
                    }
                }

                return BufferIndexes[t.Name];
            }
        }

        internal static void RebuildClass(Type classType, bool repairCollection)
        {
            if (repairCollection && !ConfigManager.Config.Context.Generated
                && !Db(classType.Name).CollectionExists(CollectionsManager.GetCollectioName(classType.Name)))
            {
                Db(classType.Name).CreateCollection(CollectionsManager.GetCollectioName(classType.Name), null);
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
            object[] RegTypes = classType.GetCustomAttributes(typeof (MongoCustomDiscriminatorType), false);

            foreach (object RegType in RegTypes)
            {
                if (RegType != null)
                {
                    var MongoCustomDiscriminatorType = (MongoCustomDiscriminatorType) RegType;
                    BsonDefaultSerializer.RegisterDiscriminator(
                        MongoCustomDiscriminatorType.Type, MongoCustomDiscriminatorType.Type.Name);
                }
            }
        }
    }
}