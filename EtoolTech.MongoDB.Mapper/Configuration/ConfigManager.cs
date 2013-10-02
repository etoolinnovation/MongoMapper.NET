using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class ConfigManager
    {
        #region Constants and Fields

        public static readonly MongoMapperConfiguration Config = MongoMapperConfiguration.GetConfig();

        public static string DatabasePrefix { get; set; }

        private static readonly Dictionary<string, CollectionElement> ConfigByObject =
            new Dictionary<string, CollectionElement>();

        private static readonly Dictionary<string, MongoClientSettings> SettingsByObject =
            new Dictionary<string, MongoClientSettings>();

        private static readonly Dictionary<string, string> UrlByObject =
            new Dictionary<string, string>();


        private static readonly Object LockObject = new Object();

        private static readonly Object LockSettingsObject = new Object();

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

        public static string DataBaseName(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Database;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return String.IsNullOrEmpty(DatabasePrefix) ? cfg.Database.Name : string.Format("{0}_{1}", DatabasePrefix, cfg.Database.Name);
            }

            return String.IsNullOrEmpty(DatabasePrefix) ? Config.Database.Name : string.Format("{0}_{1}", DatabasePrefix, Config.Database.Name);            
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


        public static void OverrideUrlString(string UrlString)
        {
            _urlString = UrlString;
        }


        public static MongoClientSettings GetClientSettings(string ObjName)
        {
            if (SettingsByObject.ContainsKey(ObjName)) return SettingsByObject[ObjName];

            string urlString = Url(ObjName);
            if (!string.IsNullOrEmpty(_urlString))
            {
                urlString = _urlString;
            }

            lock (LockSettingsObject)
            {
                if (!SettingsByObject.ContainsKey(ObjName))
                {
                    MongoClientSettings clientSettings = null;

                    if (UrlByObject.ContainsValue(urlString))
                    {
                        clientSettings = SettingsByObject[UrlByObject.First(o => o.Value == urlString).Key];
                    }
                    else
                    {
                        var url = new MongoUrl(urlString);
                        clientSettings = MongoClientSettings.FromUrl(url);
                    }

                    UrlByObject.Add(ObjName,urlString);
                    SettingsByObject.Add(ObjName, clientSettings);
                }
            }

            return SettingsByObject[ObjName];
        }


        public static string Url(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Url;
            }

            CollectionElement cfg = FindByObjName(ObjName);

            if (cfg != null)
            {
                return cfg.Server.Url;
            }

            return Config.Server.Url;
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


        public static bool UseChildIncrementalId(string ObjName)
        {
            if (MongoMapperHelper.BufferIdIncrementables[ObjName] != null)
            {
                return MongoMapperHelper.BufferIdIncrementables[ObjName].ChildsIncremenalId;
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
            if (MongoMapperHelper.BufferIdIncrementables[ObjName] != null)
            {
                return MongoMapperHelper.BufferIdIncrementables[ObjName].IncremenalId;
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