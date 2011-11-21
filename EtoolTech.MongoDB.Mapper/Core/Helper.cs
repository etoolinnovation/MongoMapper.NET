using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class Helper
    {
        private static readonly List<Type> SupportedTypesLits = new List<Type>
                                                                    {
                                                                        typeof (string),
                                                                        typeof (decimal),
                                                                        typeof (int),
                                                                        typeof (long),
                                                                        typeof (DateTime),
                                                                        typeof (bool)
                                                                    };


        private static readonly Dictionary<string, MongoCollection> Collections = new Dictionary<string, MongoCollection>();

        private static readonly Dictionary<string, List<string>> BufferPrimaryKey = new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferIndexes = new Dictionary<string, List<string>>();
        
        private static MongoServer _server;
        private static MongoDatabase _dataBase;

        private static readonly string DataBaseName = ConfigurationManager.AppSettings["DatabaseName"];
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["ConectionString"];

        public static MongoDatabase Db
        {
            get
            {
                if (_server == null)
                    _server =
                        MongoServer.Create(Context.Config == null ? ConnectionString : Context.Config.ConnectionString);

                if (_server.State != MongoServerState.Connected && _server.State != MongoServerState.Connecting)
                    _server.Connect();

                return _dataBase ??
                       (_dataBase = _server.GetDatabase(Context.Config == null ? DataBaseName : Context.Config.Database));
            }
        }

        public static MongoCollection GetCollection(string name)
        {
            name = GetCollectioName(name);

            if (Collections.ContainsKey(name))
                return Collections[name];

            MongoCollection collection = Db.GetCollection(name);
            Collections.Add(name, collection);
            return collection;
        }

        public static string GetCollectioName(string name)
        {
            if (!name.EndsWith("_Collection"))
                name = string.Format("{0}_Collection", name);
            return name;
        }

        public static void ValidateType(Type t)
        {
            if (!SupportedTypesLits.Contains(t))
                throw new TypeNotSupportedException();
        }

        public static IEnumerable<string> GetPrimaryKey(Type t)
        {
            if (!BufferPrimaryKey.ContainsKey(t.Name))
            {
                var keyAtt = (MongoKey) t.GetCustomAttributes(typeof (MongoKey), false).FirstOrDefault();
                if (keyAtt != null)
                {
                    BufferPrimaryKey.Add(t.Name, keyAtt.KeyFields.Split(',').ToList());
                }
            }

            return BufferPrimaryKey[t.Name];
        }

        private static IEnumerable<string> GetIndexes(Type t)
        {
            
             if (!BufferIndexes.ContainsKey(t.Name))
             {
                 BufferIndexes.Add(t.Name,new List<string>());
                 var indexAtt = t.GetCustomAttributes(typeof (MongoIndex), false);
               
                foreach (var index in indexAtt)
                {
                    if (index != null)
                        BufferIndexes[t.Name].Add( ((MongoIndex) index).IndexFields);
                }
                 
             }

            return BufferIndexes[t.Name];
        }

        internal static void RebuildClass(Type classType, bool repairCollection)
        {
            if (repairCollection && !Db.CollectionExists(GetCollectioName(classType.Name)) && !Context.ContextGenerated)
                Db.CreateCollection(GetCollectioName(classType.Name),null);

            RegisterCustomDiscriminatorTypes(classType);

            if (!Context.ContextGenerated || repairCollection)
            {
                foreach (string index in GetIndexes(classType))
                {
                    GetCollection(GetCollectioName(classType.Name)).EnsureIndex(index.Split(','));
                }
                GetCollection(GetCollectioName(classType.Name)).EnsureIndex(IndexKeys.Ascending(GetPrimaryKey(classType).ToArray()));
            }
        }

        private static void RegisterCustomDiscriminatorTypes(Type classType)
        {
            var RegTypes = classType.GetCustomAttributes(typeof (MongoCustomDiscriminatorType),false);

            foreach (var RegType in RegTypes)
            {
                if (RegType != null)
                {
                    MongoCustomDiscriminatorType MongoCustomDiscriminatorType = (MongoCustomDiscriminatorType) RegType;
                    BsonDefaultSerializer.RegisterDiscriminator(MongoCustomDiscriminatorType.Type, MongoCustomDiscriminatorType.Type.Name);
                }
            }
        }

    }
}