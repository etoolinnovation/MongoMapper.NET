using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey = new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly HashSet<string> CustomDiscriminatorTypes = new HashSet<string>();

        internal static readonly Dictionary<string,MongoMapperIdIncrementable> BufferIdIncrementables = new Dictionary<string, MongoMapperIdIncrementable>();

        private static readonly Object LockObjectPk = new Object();

        private static readonly Object LockObjectIndex = new Object();

        private static readonly Object LockObjectCustomDiscritminatorTypes = new Object();

        private static readonly Object LockObjectIdIncrementables = new Object();


        public static MongoDatabase Db(string objName)
        {

            string databaseName = ConfigManager.DataBaseName(objName);

            string connectionString = ConfigManager.GetConnectionString(objName);

            MongoServer server = MongoServer.Create(connectionString);

            MongoDatabase dataBase = server.GetDatabase(databaseName);
            return dataBase;
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

            lock (LockObjectPk)
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

            lock (LockObjectIndex)
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
                lock(LockObjectCustomDiscritminatorTypes)
                {
                    if (!BufferIdIncrementables.ContainsKey(classType.Name))
                    {
                        var m =
                            classType.GetCustomAttributes(typeof (MongoMapperIdIncrementable), false).FirstOrDefault();
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
            object[] regTypes = classType.GetCustomAttributes(typeof (MongoCustomDiscriminatorType), false);

            foreach (object regType in regTypes)
            {
                if (regType != null)
                {
                    var mongoCustomDiscriminatorType = (MongoCustomDiscriminatorType) regType;
                    BsonDefaultSerializer.RegisterDiscriminator(
                        mongoCustomDiscriminatorType.Type, mongoCustomDiscriminatorType.Type.Name);
                }
            }
        }
    }
}