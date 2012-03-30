using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class 
        ConfigManager
    {
        public static readonly MongoMapperConfiguration Config = MongoMapperConfiguration.GetConfig();

        private static readonly Dictionary<string, CollectionElement> ConfigByObject = new Dictionary<string, CollectionElement>();

        private static readonly Object LockObject = new Object();

        private static bool _setupLoaded = false;

        internal static bool IsReplicaSet { get; set; }
        internal static int? ActiveServers { get; set; }

        public static string GetConnectionString(string objName)
        {
            string loginString = "";
            string userName = ConfigManager.UserName(objName);

            if (!String.IsNullOrEmpty(userName))
            {
                loginString = String.Format("{0}:{1}@", userName, ConfigManager.PassWord(objName));
            }

            string databaseName = ConfigManager.DataBaseName(objName);

            string host = ConfigManager.Host(objName);            

            string hostsStrings = "";
            if (host.Contains(","))
            {
                string[] hostList = host.Split(',');
                hostsStrings = hostList.Aggregate(hostsStrings, (current, h) => current + (h + ","));
                if (!String.IsNullOrEmpty(hostsStrings)) hostsStrings = hostsStrings.Remove(hostsStrings.Length - 1, 1);
                IsReplicaSet = true;
            }
            else
            {
                hostsStrings = host;
            }

            string replicaOptions = "";
            
            string replicaSetName = ConfigManager.ReplicaSetName(objName);
            if (!String.IsNullOrEmpty(replicaSetName))
                replicaOptions = string.Format("replicaSet={0}", replicaSetName);

            if (ConfigManager.MinReplicaServersToWrite(objName) != 0)
                replicaOptions += String.Format(";w={0}", ConfigManager.MinReplicaServersToWrite(objName)); 

            if (ConfigManager.BalancedReading(objName))
                replicaOptions += String.Format(";slaveOk=true");

            if (replicaOptions.StartsWith(";")) replicaOptions = replicaOptions.Remove(0, 1);

            if (!String.IsNullOrEmpty(replicaOptions)) replicaOptions += ";";
            

            string connectionString = String.Format("mongodb://{4}{0}/{5}?{1}maxpoolsize={2};waitQueueTimeout={3}ms;safe={6};fsync={7}",
                                                    hostsStrings, replicaOptions,
                                                    ConfigManager.PoolSize(objName),
                                                    ConfigManager.WaitQueueTimeout(objName) * 1000, loginString, databaseName,
                                                    ConfigManager.SafeMode(objName).ToString(CultureInfo.InvariantCulture).ToLower(),
                                                    ConfigManager.FSync(objName).ToString(CultureInfo.InvariantCulture).ToLower());
            return connectionString;
        }

        private static string CleanObjName(string objName)
        {
            if (objName.EndsWith("_Collection"))
            {
                objName = objName.Replace("_Collection", "");
            }
            return objName;
        }

        private static CollectionElement FindByObjName(string objName)
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

            objName = CleanObjName(objName);

            return ConfigByObject.ContainsKey(objName) ? ConfigByObject[objName] : null;
        }

        public static string DataBaseName(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Database;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.Name;

            return Config.Database.Name;

        }

        public static string Host(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Host;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.Host;

            return Config.Server.Host;
        }


        public static string ReplicaSetName(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.ReplicaSetName;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.ReplicaSetName;

            return Config.Server.ReplicaSetName;
        }
        
        public static bool BalancedReading(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.BalancedReading;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.BalancedReading;

            return Config.Server.BalancedReading;
        }


        public static int MinReplicaServersToWrite(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.MinReplicaServersToWrite;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.MinReplicaServersToWrite;

            return Config.Server.MinReplicaServersToWrite;
        }
  

        public static int PoolSize(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.PoolSize;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.PoolSize;

            return Config.Server.PoolSize;
        }

        public static string UserName(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.UserName;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.User;

            return Config.Database.User;
        }

        public static string PassWord(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.PassWord;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.Password;

            return Config.Database.Password;
        }

        public static int WaitQueueTimeout(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.WaitQueueTimeout;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.WaitQueueTimeout;

            return Config.Server.WaitQueueTimeout;
        }

        public static int MaxDocumentSize(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.MaxDocumentSize;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.MaxDocumentSize;

            return Config.Context.MaxDocumentSize;

        }

        public static bool SafeMode(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.SafeMode;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.SafeMode;

            return Config.Context.SafeMode;

        }

        public static bool FSync(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.FSync;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.FSync;

            return Config.Context.FSync;

        }

         public static bool ExceptionOnDuplicateKey(string objName)
         {
             if (CustomContext.Config != null) return CustomContext.Config.ExceptionOnDuplicateKey;

             CollectionElement cfg = FindByObjName(objName);

             if (cfg != null) return cfg.Context.ExceptionOnDuplicateKey;

             return Config.Context.ExceptionOnDuplicateKey;

         }

        public static bool EnableOriginalObject(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.EnableOriginalObject;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.EnableOriginalObject;

            return Config.Context.EnableOriginalObject;

        }

        public static bool UseIncrementalId(string objName)
        {
            if (Helper.BufferIdIncrementables[objName] != null)
                return Helper.BufferIdIncrementables[objName].IncremenalId;
            
            if (CustomContext.Config != null) return CustomContext.Config.UseIncrementalId;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.UseIncrementalId;

            return Config.Context.UseIncrementalId;

        }
		
		public static bool UseChildIncrementalId(string objName)
		{

            if (Helper.BufferIdIncrementables[objName] != null)
                return Helper.BufferIdIncrementables[objName].ChildsIncremenalId;

			if (CustomContext.Config != null) return CustomContext.Config.UseChildIncrementalId;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.UseChidlsIncrementalId;

            return Config.Context.UseChidlsIncrementalId;
		}
    }
}