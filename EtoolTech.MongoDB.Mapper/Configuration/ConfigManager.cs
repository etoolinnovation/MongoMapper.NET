using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class ConfigManager
    {
        #region Constants and Fields

        public static readonly MongoMapperConfiguration Config = MongoMapperConfiguration.GetConfig();

        private static readonly Dictionary<string, CollectionElement> ConfigByObject =
            new Dictionary<string, CollectionElement>();

        private static readonly Object LockObject = new Object();

        private static bool _setupLoaded;

        #endregion

        #region Public Properties

        public static TextWriter Out { get; set; }

        #endregion

        #region Properties

        internal static int? ActiveServers { get; set; }

        internal static bool IsReplicaSet { get; set; }

        #endregion

        #region Public Methods

        private static string _urlString = String.Empty;

        public static bool BalancedReading(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.BalancedReading;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.BalancedReading;
            }

            return Config.Server.BalancedReading;
        }

        public static string DataBaseName(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Database;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Database.Name;
            }

            return Config.Database.Name;
        }

        public static bool EnableOriginalObject(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.EnableOriginalObject;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.EnableOriginalObject;
            }

            return Config.Context.EnableOriginalObject;
        }

        public static bool ExceptionOnDuplicateKey(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.ExceptionOnDuplicateKey;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.ExceptionOnDuplicateKey;
            }

            return Config.Context.ExceptionOnDuplicateKey;
        }

        public static bool Journal(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Journal;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.Journal;
            }

            return Config.Context.Journal;
        }

        public static void OverrideUrlString(string urlString)
        {
            _urlString = urlString;
        }


        public static MongoClientSettings GetClientSettings(string ObjName)
        {
            if (!string.IsNullOrEmpty(_urlString))
            {
                var builder = new MongoUrlBuilder(_urlString);
                MongoUrl url = builder.ToMongoUrl();
                return MongoClientSettings.FromUrl(url);
            }

            var settigs = new MongoClientSettings();

            string databaseName = DataBaseName(ObjName);
            string userName = UserName(ObjName);

            if (!String.IsNullOrEmpty(userName))
            {
                var credentials = new MongoCredentials(MongoAuthenticationProtocol.Strongest,
                                                       new MongoInternalIdentity(databaseName, userName),
                                                       new PasswordEvidence(PassWord(ObjName)));
                settigs.CredentialsStore.Add(credentials);
            }

            string host = Host(ObjName);


            if (host.Contains(","))
            {
                var servers = new List<MongoServerAddress>();
                string[] hostList = host.Split(',');
                foreach (string h in hostList)
                {
                    if (h.Contains(":"))
                    {
                        string[] hostPort = h.Split(':');
                        servers.Add(new MongoServerAddress(hostPort[0], int.Parse(hostPort[1])));
                    }
                    else
                    {
                        servers.Add(new MongoServerAddress(h));
                    }
                }

                settigs.Servers = servers;
            }
            else
            {
                settigs.Servers.ToList().Add(new MongoServerAddress(host));
            }

            var wc = new WriteConcern();

            string replicaSetName = ReplicaSetName(ObjName);
            if (!String.IsNullOrEmpty(replicaSetName))
            {
                settigs.ReplicaSetName = replicaSetName;
            }

            if (MinReplicaServersToWrite(ObjName) != 0)
            {
                wc.W = MinReplicaServersToWrite(ObjName);
            }

            settigs.ReadPreference = BalancedReading(ObjName)
                                         ? ReadPreference.SecondaryPreferred
                                         : ReadPreference.PrimaryPreferred;

            if (Journal(ObjName))
            {
                wc.Journal = true;
            }

            settigs.WriteConcern = wc;
            if (!SafeMode(ObjName) && settigs.WriteConcern.Journal == null && settigs.WriteConcern.W == null)
            {
                wc.W = 0;
            }

            settigs.MaxConnectionPoolSize = PoolSize(ObjName);
            settigs.WaitQueueTimeout = TimeSpan.FromSeconds(WaitQueueTimeout(ObjName));


            return settigs;
        }


        public static string Host(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Host;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.Host;
            }

            return Config.Server.Host;
        }

        public static int MaxDocumentSize(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.MaxDocumentSize;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.MaxDocumentSize;
            }

            return Config.Context.MaxDocumentSize;
        }

        public static int MinReplicaServersToWrite(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.MinReplicaServersToWrite;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.MinReplicaServersToWrite;
            }

            return Config.Server.MinReplicaServersToWrite;
        }

        public static string PassWord(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.PassWord;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Database.Password;
            }

            return Config.Database.Password;
        }

        public static int PoolSize(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.PoolSize;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.PoolSize;
            }

            return Config.Server.PoolSize;
        }

        public static string ReplicaSetName(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.ReplicaSetName;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.ReplicaSetName;
            }

            return Config.Server.ReplicaSetName;
        }

        public static bool SafeMode(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.SafeMode;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.SafeMode;
            }

            return Config.Context.SafeMode;
        }

        public static bool UseChildIncrementalId(string ObjName)
        {
            if (Helper.BufferIdIncrementables[ObjName] != null)
            {
                return Helper.BufferIdIncrementables[ObjName].ChildsIncremenalId;
            }

            if (CustomContext.Config != null)
            {
                return CustomContext.Config.UseChildIncrementalId;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.UseChidlsIncrementalId;
            }

            return Config.Context.UseChidlsIncrementalId;
        }

        public static bool UseIncrementalId(string ObjName)
        {
            if (Helper.BufferIdIncrementables[ObjName] != null)
            {
                return Helper.BufferIdIncrementables[ObjName].IncremenalId;
            }

            if (CustomContext.Config != null)
            {
                return CustomContext.Config.UseIncrementalId;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Context.UseIncrementalId;
            }

            return Config.Context.UseIncrementalId;
        }

        public static string UserName(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.UserName;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Database.User;
            }

            return Config.Database.User;
        }

        public static int WaitQueueTimeout(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.WaitQueueTimeout;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.WaitQueueTimeout;
            }

            return Config.Server.WaitQueueTimeout;
        }

        #endregion

        #region Methods

        private static string CleanObjName(string ObjName)
        {
            if (ObjName.EndsWith("_Collection"))
            {
                ObjName = ObjName.Replace("_Collection", "");
            }
            return ObjName;
        }

        private static CollectionElement FindByObjName(string ObjName)
        {
            if (!_setupLoaded)
            {
                lock (LockObject)
                {
                    if (!_setupLoaded)
                    {
                        foreach (CollectionElement collection in Config.CollectionConfig)
                        {
                            ConfigByObject.Add(collection.Name, collection);
                        }
                        _setupLoaded = true;
                    }
                }
            }

            ObjName = CleanObjName(ObjName);

            return ConfigByObject.ContainsKey(ObjName) ? ConfigByObject[ObjName] : null;
        }

        #endregion
    }
}