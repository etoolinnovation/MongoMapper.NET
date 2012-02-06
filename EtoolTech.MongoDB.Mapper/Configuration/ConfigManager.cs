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

        private static readonly Dictionary<string, CollectionElement> configByObject = new Dictionary<string, CollectionElement>();

        private static readonly Object _lockObject = new Object();

        private static bool SetupLoaded = false;

        public static string GetConnectionString(string objName)
        {
            string LoginString = "";
            string userName = ConfigManager.UserName(objName);

            if (!String.IsNullOrEmpty(userName))
            {
                LoginString = String.Format("{0}:{1}@", userName, ConfigManager.PassWord(objName));
            }

            string DatabaseName = ConfigManager.DataBaseName(objName);

            string connectionString = String.Format("mongodb://{4}{0}:{1}/{5}?connect=direct;maxpoolsize={2};waitQueueTimeout={3}ms;safe={6};fsync={7}",
                                                    ConfigManager.Host(objName), ConfigManager.Port(objName),
                                                    ConfigManager.PoolSize(objName),
                                                    ConfigManager.WaitQueueTimeout(objName) * 1000, LoginString, DatabaseName,
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

        private static CollectionElement FindByObjName(string ObjName)
        {
            if (!SetupLoaded)
            {
                lock (_lockObject)
                {
                    if (!SetupLoaded)
                    {                        
                        foreach (CollectionElement collection in Config.CollectionConfig)
                        {
                            configByObject.Add(collection.Name, collection);
                        }
                        SetupLoaded = true;
                    }
                }
            }

            ObjName = CleanObjName(ObjName);

            return configByObject.ContainsKey(ObjName) ? configByObject[ObjName] : null;
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

        public static int Port(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Port;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.Port;

            return Config.Server.Port;
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
            if (CustomContext.Config != null) return CustomContext.Config.UseIncrementalId;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.UseIncrementalId;

            return Config.Context.UseIncrementalId;

        }
		
		public static bool UseChildIncrementalId(string objName)
		{
			if (CustomContext.Config != null) return CustomContext.Config.UseChildIncrementalId;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.UseChidlsIncrementalId;

            return Config.Context.UseChidlsIncrementalId;
		}
    }
}