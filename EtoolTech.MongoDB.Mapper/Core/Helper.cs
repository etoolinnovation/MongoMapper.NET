using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
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

        private static readonly MongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();

        private static readonly string DataBaseName = config.Database.Name;
        private static readonly string Host = config.Server.Host;
        private static readonly int Port = config.Server.Port;
        private static readonly int PoolSize = config.Server.PoolSize;
        private static readonly string UserName = config.Database.User;
        private static readonly string PassWord = config.Database.Password;


        public static MongoDatabase Db
        {
            get
            {

                if (_server == null)
                {
                    MongoServerSettings ServerSettings = new MongoServerSettings();
                    string userName = Context.Config == null ? UserName : Context.Config.UserName;

                    if (!String.IsNullOrEmpty(userName))
                        ServerSettings.DefaultCredentials = new MongoCredentials(userName, Context.Config == null ? PassWord : Context.Config.PassWord);

                    ServerSettings.Server = new MongoServerAddress(Context.Config == null ? Host : Context.Config.Host, Context.Config == null ? Port : Context.Config.Port);
                    ServerSettings.MaxConnectionPoolSize = Context.Config == null ? PoolSize : Context.Config.PoolSize;

                    _server = MongoServer.Create(ServerSettings);
                }

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

        //TODO: Pendiente de refactor, meter en un buffer o usarlo siempre tipado.
        public static MongoCollection<T> GetCollection<T>(string name)
        {
            name = GetCollectioName(name);

            MongoCollection<T> collection = Db.GetCollection<T>(name);
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