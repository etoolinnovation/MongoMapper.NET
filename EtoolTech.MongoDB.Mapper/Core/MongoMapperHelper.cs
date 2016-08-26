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
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperHelper
    {
        #region Constants and Fields

        internal static readonly Dictionary<string, MongoMapperIdIncrementable> BufferIdIncrementables =
            new Dictionary<string, MongoMapperIdIncrementable>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, string> BufferTTLIndex = new Dictionary<string, string>();

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey = new Dictionary<string, List<string>>();

        internal static readonly Dictionary<string, Dictionary<string, object>> BufferDefaultValues = new Dictionary<string, Dictionary<string, object>>();

        internal static readonly Dictionary<string, Dictionary<string, string>> BufferCustomFieldNames = new Dictionary<string, Dictionary<string, string>>(); 

        private static readonly HashSet<string> CustomDiscriminatorTypes = new HashSet<string>();

        private static readonly Object LockObjectCustomDiscritminatorTypes = new Object();

        private static readonly Object LockObjectIdIncrementables = new Object();

        private static readonly Object LockObjectIndex = new Object();

        private static readonly Object LockObjectTTL = new Object();

        private static readonly Object LockObjectPk = new Object();

        private static readonly Object LockObjectDefaults = new Object();

        private static readonly Object LockObjectCustomFieldNames = new Object();

        private static readonly Object LockObjectCustomCollectionNames = new Object();

        private static readonly HashSet<Type> SupportedTypesLits = new HashSet<Type> {typeof (string), typeof (decimal), typeof (int), typeof (long), typeof (DateTime), typeof (bool)};

        #endregion
      
        #region Public Methods

        public static IMongoDatabase Db(string ObjName)
        {
            return Db(ObjName, false);
        }

        public static IMongoDatabase Db(string ObjName, bool Primary)
        {
            string databaseName = ConfigManager.DataBaseName(ObjName);

            MongoClientSettings settings = ConfigManager.GetClientSettings(ObjName);

            if (Primary) settings.ReadPreference = ReadPreference.Primary;

            var client = new MongoClient(settings);
            
            var dataBase = client.GetDatabase(databaseName);
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

        public static string GetTTLIndex(Type T)
        {
            if (BufferTTLIndex.ContainsKey(T.Name))
            {
                return BufferTTLIndex[T.Name];
            }

            lock (LockObjectTTL)
            {
                if (!BufferTTLIndex.ContainsKey(T.Name))
                {
                    var keyAtt = (MongoTTLIndex)T.GetCustomAttributes(typeof(MongoTTLIndex), false).FirstOrDefault();
                    if (keyAtt != null)
                    {                       
                        BufferTTLIndex.Add(T.Name, keyAtt.IndexField + "," + keyAtt.Seconds);
                    }
                    else
                    {
                        BufferTTLIndex.Add(T.Name, string.Empty);
                    }
                }

                return BufferTTLIndex[T.Name];
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
            
            //MongoCollectionName
            if (!CollectionsManager.CustomCollectionsName.ContainsKey(ClassType.Name))
            {
				lock(LockObjectCustomCollectionNames)
                {
                     if (!CollectionsManager.CustomCollectionsName.ContainsKey(ClassType.Name))
                     {
                            var colNameAtt = (MongoCollectionName) ClassType.GetCustomAttributes(typeof (MongoCollectionName), false).FirstOrDefault();
                            if (colNameAtt != null)
                            {
                                CollectionsManager.CustomCollectionsName.Add(ClassType.Name,colNameAtt);
                            }
                     }
                }
            }
            
            
            if ((RepairCollection || !ConfigManager.Config.Context.Generated)
                && !CollectionsManager.CollectionExists(CollectionsManager.GetCollectioName(ClassType.Name)))
            {
                Db(ClassType.Name).CreateCollectionAsync((CollectionsManager.GetCollectioName(ClassType.Name))).GetAwaiter().GetResult();
            }

            lock (LockObjectCustomDiscritminatorTypes)
            {
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
            }

            lock (LockObjectIdIncrementables)
            {
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
            }

            if (!BufferDefaultValues.ContainsKey(ClassType.Name))
            {
                lock (LockObjectDefaults)
                {
                    if (!BufferDefaultValues.ContainsKey(ClassType.Name))
                    {
                        BufferDefaultValues.Add(ClassType.Name,new Dictionary<string, object>());
                        var properties = ClassType.GetProperties().Where(P => P.GetCustomAttributes(typeof(BsonDefaultValueAttribute), true).Count() != 0);
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

     
            if (!ConfigManager.Config.Context.Generated || RepairCollection)
            {

                CreateIndexes(ClassType);
            }
        }

        internal static void CreateIndexes(Type ClassType)
        {
            var existingIndexNames = GetExistinIndexNames(ClassType);

            foreach (string index in GetIndexes(ClassType))
            {
                if (index.StartsWith("2D|"))
                {
                    var mongoIndex = Builders<BsonDocument>.IndexKeys.Geo2D(MongoMapperHelper.ConvertFieldName(ClassType.Name, index.Split('|')[1]).Trim());
                    var indexName = "2D" + "_" + index.Split('|')[1];

                    if (!existingIndexNames.Contains(indexName))
                    {
                        Console.WriteLine("CREATING INDEX IN" + ClassType.Name + " => " + indexName);
                        CollectionsManager.GetCollection<BsonDocument>(ClassType.Name)
                            .Indexes.CreateOneAsync(mongoIndex, new CreateIndexOptions() {Name = indexName})
                            .GetAwaiter()
                            .GetResult();
                    }
                }
                else if (index.StartsWith("2DSphere|"))
                {
                    var indexName = "2DSphere" + "_" + index.Split('|')[1];

                    if (!existingIndexNames.Contains(indexName))
                    {

                        Console.WriteLine("CREATING INDEX IN" + ClassType.Name + " => " + indexName);
                        var mongoIndex =
                            Builders<BsonDocument>.IndexKeys.Geo2DSphere(
                                MongoMapperHelper.ConvertFieldName(ClassType.Name, index.Split('|')[1]).Trim());

                        CollectionsManager.GetCollection<BsonDocument>(ClassType.Name)
                            .Indexes.CreateOneAsync(mongoIndex, new CreateIndexOptions() {Name = indexName})
                            .GetAwaiter()
                            .GetResult();
                    }
                }
                else
                {
                    var indexFieldnames = MongoMapperHelper.ConvertFieldName(ClassType.Name, index.Split(',').ToList()).Select(IndexField => IndexField.Trim());

                    var fieldnames = indexFieldnames as IList<string> ?? indexFieldnames.ToList();

                    if (fieldnames.Any())
                    {
                        
                        var indexName = "IX" + "_" + string.Join("_", fieldnames);

                        if (!existingIndexNames.Contains(indexName))
                        {
                            Console.WriteLine("CREATING INDEX IN" + ClassType.Name + " => " + indexName);

                            var indexFields = Builders<BsonDocument>.IndexKeys.Ascending(fieldnames.First());
                            indexFields = fieldnames.Skip(1)
                                .Aggregate(indexFields, (Current, FieldName) => Current.Ascending(FieldName));

                            CollectionsManager.GetCollection<BsonDocument>(ClassType.Name)
                                .Indexes.CreateOneAsync(indexFields, new CreateIndexOptions() {Name = indexName})
                                .GetAwaiter()
                                .GetResult();
                        }
                    }
                }
            }

            string[] pk = GetPrimaryKey(ClassType).ToArray();
            if (pk.Count(K => K == "m_id") == 0)
            {
                var indexFieldnames = MongoMapperHelper.ConvertFieldName(ClassType.Name, pk.ToList()).Select(PkField => PkField.Trim());

                var fieldnames = indexFieldnames as IList<string> ?? indexFieldnames.ToList();
                if (fieldnames.Any())
                {
                    var indexName = "PK_" + string.Join("_", fieldnames);

                    if (!existingIndexNames.Contains(indexName))
                    {
                        Console.WriteLine("CREATING INDEX IN" + ClassType.Name + " => " + indexName);

                        var indexFields = Builders<BsonDocument>.IndexKeys.Ascending(fieldnames.First());

                        indexFields = fieldnames.Skip(1)
                            .Aggregate(indexFields, (Current, FieldName) => Current.Ascending(FieldName));

                        CollectionsManager.GetCollection<BsonDocument>(ClassType.Name)
                            .Indexes.CreateOneAsync(indexFields,
                                new CreateIndexOptions() {Unique = true, Name = indexName})
                            .GetAwaiter()
                            .GetResult();
                    }
                }
            }

            string ttlIndex = GetTTLIndex(ClassType);
            if (ttlIndex != string.Empty)
            {
                var tmpIndex = ttlIndex.Split(',');
                var indexName = "TTL_" + tmpIndex[0].Trim();

                if (!existingIndexNames.Contains(indexName))
                {
                    Console.WriteLine("CREATING INDEX IN" + ClassType.Name + " => " + indexName);

                    var keys = Builders<BsonDocument>.IndexKeys.Ascending(tmpIndex[0].Trim());
                    CollectionsManager.GetCollection<BsonDocument>(ClassType.Name).Indexes.CreateOneAsync(
                        keys,
                        new CreateIndexOptions()
                        {
                            Name = indexName,
                            ExpireAfter = TimeSpan.FromSeconds(int.Parse(tmpIndex[1].Trim()))
                        })
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }

        internal static List<string> GetExistinIndexNames(Type ClassType)
        {
            var existingIndexNames =
                CollectionsManager.GetCollection<BsonDocument>(ClassType.Name)
                    .Indexes.ListAsync().GetAwaiter().GetResult().ToList()
                    .Select(Index => Index["name"].ToString())
                    .ToList();
            return existingIndexNames;
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

                    var geoSphereIndexAtt =
                     (MongoGeo2DSphereIndex)T.GetCustomAttributes(typeof(MongoGeo2DSphereIndex), false).FirstOrDefault();
                    if (geoSphereIndexAtt != null)
                    {
                        BufferIndexes[T.Name].Add("2DSphere|" + geoSphereIndexAtt.IndexField);
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


        public static object GetFieldDefaultValue(string ObjName, string FieldName)
        {
            if (BufferDefaultValues.ContainsKey(ObjName) && BufferDefaultValues[ObjName].ContainsKey(FieldName))
                return BufferDefaultValues[ObjName][FieldName];

            return null;
        }

        public static string ConvertFieldName(string ObjName, string FieldName)
        {
            if (FieldName == "m_id") return "_id";
            if (BufferCustomFieldNames.ContainsKey(ObjName) && BufferCustomFieldNames[ObjName].ContainsKey(FieldName))
                return BufferCustomFieldNames[ObjName][FieldName];

            return FieldName;
        }

        public static List<string> ConvertFieldName(string ObjName, List<string> FieldNames)
        {
            return FieldNames.Select(Field => ConvertFieldName(ObjName, Field)).ToList();
        }
    }
}