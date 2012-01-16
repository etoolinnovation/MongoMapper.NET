using System;
using System.Collections.Generic;
using System.Linq;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class ConfigManager
    {
        internal static readonly MongoMapperConfiguration Config = MongoMapperConfiguration.GetConfig();

        private static readonly Dictionary<string, CollectionElement> configByObject = new Dictionary<string, CollectionElement>();

        private static readonly Object _lockObject = new Object();

        private static bool SetupLoaded = false;

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

        internal static string DataBaseName(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Database;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.Name;

            return Config.Database.Name;

        }

        internal static string Host(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Host;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.Host;

            return Config.Server.Host;
        }

        internal static int Port(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.Port;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.Port;

            return Config.Server.Port;
        }

        internal static int PoolSize(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.PoolSize;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.PoolSize;

            return Config.Server.PoolSize;
        }

        internal static string UserName(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.UserName;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.User;

            return Config.Database.User;
        }

        internal static string PassWord(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.PassWord;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Database.Password;

            return Config.Database.Password;
        }

        internal static int WaitQueueTimeout(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.WaitQueueTimeout;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Server.WaitQueueTimeout;

            return Config.Server.WaitQueueTimeout;
        }


        internal static int MaxDocumentSize(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.MaxDocumentSize;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.MaxDocumentSize;

            return Config.Context.MaxDocumentSize;

        }

        internal static bool SafeMode(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.SafeMode;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.SafeMode;

            return Config.Context.SafeMode;

        }

        internal static bool FSync(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.FSync;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.FSync;

            return Config.Context.FSync;

        }

         internal static bool ExceptionOnDuplicateKey(string objName)
         {
             if (CustomContext.Config != null) return CustomContext.Config.ExceptionOnDuplicateKey;

             CollectionElement cfg = FindByObjName(objName);

             if (cfg != null) return cfg.Context.ExceptionOnDuplicateKey;

             return Config.Context.ExceptionOnDuplicateKey;

         }

        internal static bool EnableOriginalObject(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.EnableOriginalObject;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.EnableOriginalObject;

            return Config.Context.EnableOriginalObject;

        }

        internal static bool UserIncrementalId(string objName)
        {
            if (CustomContext.Config != null) return CustomContext.Config.UserIncrementalId;

            CollectionElement cfg = FindByObjName(objName);

            if (cfg != null) return cfg.Context.UserIncrementalId;

            return Config.Context.UserIncrementalId;

        }
    }
}