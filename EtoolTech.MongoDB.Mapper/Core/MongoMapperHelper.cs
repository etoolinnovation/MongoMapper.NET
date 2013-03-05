using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperHelper
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoMapperIdIncrementable> BufferIdIncrementables =
            new Dictionary<string, MongoMapperIdIncrementable>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey = new Dictionary<string, List<string>>();

        internal static readonly Dictionary<string, Dictionary<string, object>> BufferDefaultValues = new Dictionary<string, Dictionary<string, object>>();

        internal static readonly Dictionary<string, Dictionary<string, string>> BufferCustomFieldNames = new Dictionary<string, Dictionary<string, string>>(); 

        private static readonly HashSet<string> CustomDiscriminatorTypes = new HashSet<string>();

        private static readonly Object LockObjectCustomDiscritminatorTypes = new Object();

        private static readonly Object LockObjectIdIncrementables = new Object();

        private static readonly Object LockObjectIndex = new Object();

        private static readonly Object LockObjectPk = new Object();

        private static readonly Object LockObjectDefaults = new Object();

        private static readonly Object LockObjectCustomFieldNames = new Object();

        private static readonly HashSet<Type> SupportedTypesLits = new HashSet<Type> {typeof (string), typeof (decimal), typeof (int), typeof (long), typeof (DateTime), typeof (bool)};

        #endregion

      

        #region Public Methods

        public static MongoDatabase Db(string ObjName)
        {
            return Db(ObjName, false);
        }

        public static MongoDatabase Db(string ObjName, bool Primary)
        {
            string databaseName = ConfigManager.DataBaseName(ObjName);

            MongoClientSettings settings = ConfigManager.GetClientSettings(ObjName);

            if (Primary) settings.ReadPreference = ReadPreference.Primary;

            var client = new MongoClient(settings);

            MongoServer server = client.GetServer();

            MongoDatabase dataBase = server.GetDatabase(databaseName);
            return dataBase;
        }

        public static IEnumerable<string> GetPrimaryKey(Type T)
        {
            if (BufferPrimaryKey.ContainsKey(T.Name))
            {
                return BufferPrimaryKey[T.Name];
            }

            lock (LockObjectPk)
            {
                if (!BufferPrimaryKey.ContainsKey(T.Name))
                {
                    var keyAtt = (MongoKey) T.GetCustomAttributes(typeof (MongoKey), false).FirstOrDefault();
                    if (keyAtt != null)
                    {
                        if (String.IsNullOrEmpty(keyAtt.KeyFields))
                        {
                            keyAtt.KeyFields = "m_id";
                        }
                        BufferPrimaryKey.Add(T.Name, keyAtt.KeyFields.Split(',').ToList());
                    }
                    else
                    {
                        BufferPrimaryKey.Add(T.Name, new List<string> {"m_id"});
                    }
                }

                return BufferPrimaryKey[T.Name];
            }
        }

        public static void ValidateType(Type T)
        {
            if (!SupportedTypesLits.Contains(T))
            {
                throw new TypeNotSupportedException(T.Name);
            }
        }

        #endregion

        #region Methods

        internal static void RebuildClass(Type ClassType, bool RepairCollection)
        {
            if ((RepairCollection || !ConfigManager.Config.Context.Generated)
                && !Db(ClassType.Name).CollectionExists(CollectionsManager.GetCollectioName(ClassType.Name)))
            {
                Db(ClassType.Name).CreateCollection(CollectionsManager.GetCollectioName(ClassType.Name), null);
            }

            if (!CustomDiscriminatorTypes.Contains(ClassType.Name))
            {
                lock (LockObjectCustomDiscritminatorTypes)
                {
                    if (!CustomDiscriminatorTypes.Contains(ClassType.Name))
                    {
                        RegisterCustomDiscriminatorTypes(ClassType);
                        CustomDiscriminatorTypes.Add(ClassType.Name);
                    }
                }
            }

            if (!BufferIdIncrementables.ContainsKey(ClassType.Name))
            {
                lock (LockObjectIdIncrementables)
                {
                    if (!BufferIdIncrementables.ContainsKey(ClassType.Name))
                    {
                        object m =
                            ClassType.GetCustomAttributes(typeof (MongoMapperIdIncrementable), false).FirstOrDefault();
                        if (m == null)
                        {
                            BufferIdIncrementables.Add(ClassType.Name, null);
                        }
                        else
                        {
                            BufferIdIncrementables.Add(ClassType.Name, (MongoMapperIdIncrementable) m);
                        }
                    }
                }
            }

            if (!BufferDefaultValues.ContainsKey(ClassType.Name))
            {
                lock (LockObjectDefaults)
                {
                    if (!BufferDefaultValues.ContainsKey(ClassType.Name))
                    {
                        BufferDefaultValues.Add(ClassType.Name,new Dictionary<string, object>());
                        var properties = ClassType.GetProperties().Where(p => p.GetCustomAttributes(typeof(BsonDefaultValueAttribute), true).Count() != 0);
                        foreach (PropertyInfo propertyInfo in properties)
                        {
                            var att = (BsonDefaultValueAttribute)propertyInfo.GetCustomAttributes(typeof(BsonDefaultValueAttribute), true).FirstOrDefault();
                            if (att != null)
                                BufferDefaultValues[ClassType.Name].Add(propertyInfo.Name,att.DefaultValue);
                        }
                    }
                }
            }


            if (!BufferCustomFieldNames.ContainsKey(ClassType.Name))
            {
                lock (LockObjectCustomFieldNames)
                {
                    if (!BufferCustomFieldNames.ContainsKey(ClassType.Name))
                    {
                        BufferCustomFieldNames.Add(ClassType.Name, new Dictionary<string, string>());
                        var properties = ClassType.GetProperties().Where(p => p.GetCustomAttributes(typeof(BsonElementAttribute), true).Count() != 0);
                        foreach (PropertyInfo propertyInfo in properties)
                        {
                            var att = (BsonElementAttribute)propertyInfo.GetCustomAttributes(typeof(BsonElementAttribute), true).FirstOrDefault();
                            if (att != null)
                                BufferCustomFieldNames[ClassType.Name].Add(propertyInfo.Name, att.ElementName);
                        }
                    }
                }
            }


            //TODO: MongoCollectionName

            if (!ConfigManager.Config.Context.Generated || RepairCollection)
            {
                foreach (string index in GetIndexes(ClassType))
                {
                    if (index.StartsWith("2D|"))
                    {
                        CollectionsManager.GetCollection(
                            CollectionsManager.GetCollectioName(ClassType.Name)).EnsureIndex(
                                IndexKeys.GeoSpatial(MongoMapperHelper.ConvertFieldName(ClassType.Name,index.Split('|')[1])));
                    }
                    else
                    {
                        CollectionsManager.GetCollection(
                            CollectionsManager.GetCollectioName(ClassType.Name)).EnsureIndex(MongoMapperHelper.ConvertFieldName(ClassType.Name,index.Split(',').ToList()).ToArray());
                    }
                }

                string[] pk = GetPrimaryKey(ClassType).ToArray();
                if (pk.Count(k => k == "m_id") == 0)
                {
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(ClassType.Name)).EnsureIndex(
                        IndexKeys.Ascending(MongoMapperHelper.ConvertFieldName(ClassType.Name,pk.ToList()).ToArray()), IndexOptions.SetUnique(true));
                }
            }
        }

        private static IEnumerable<string> GetIndexes(Type T)
        {
            if (BufferIndexes.ContainsKey(T.Name))
            {
                return BufferIndexes[T.Name];
            }

            lock (LockObjectIndex)
            {
                if (!BufferIndexes.ContainsKey(T.Name))
                {
                    BufferIndexes.Add(T.Name, new List<string>());
                    object[] indexAtt = T.GetCustomAttributes(typeof (MongoIndex), false);

                    foreach (object index in indexAtt)
                    {
                        if (index != null)
                        {
                            BufferIndexes[T.Name].Add(((MongoIndex) index).IndexFields);
                        }
                    }

                    var geoindexAtt =
                        (MongoGeo2DIndex) T.GetCustomAttributes(typeof (MongoGeo2DIndex), false).FirstOrDefault();
                    if (geoindexAtt != null)
                    {
                        BufferIndexes[T.Name].Add("2D|" + geoindexAtt.IndexField);
                    }
                }

                return BufferIndexes[T.Name];
            }
        }

        private static void RegisterCustomDiscriminatorTypes(Type ClassType)
        {
            object[] regTypes = ClassType.GetCustomAttributes(typeof (MongoCustomDiscriminatorType), false);

            foreach (object regType in regTypes)
            {
                if (regType != null)
                {
                    var mongoCustomDiscriminatorType = (MongoCustomDiscriminatorType) regType;
                    BsonSerializer.RegisterDiscriminator(
                        mongoCustomDiscriminatorType.Type, mongoCustomDiscriminatorType.Type.Name);
                }
            }
        }

        #endregion

        public static string ConvertFieldName(string ObjName, string FieldName)
        {
            if (BufferCustomFieldNames.ContainsKey(ObjName) && BufferCustomFieldNames[ObjName].ContainsKey(FieldName))
                return BufferCustomFieldNames[ObjName][FieldName];

            return FieldName;
        }

        public static List<string> ConvertFieldName(string ObjName, List<string> FieldNames)
        {
            return FieldNames.Select(field => ConvertFieldName(ObjName, field)).ToList();
        }
    }
}