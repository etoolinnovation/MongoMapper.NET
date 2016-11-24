using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class ConfigManager
    {
        #region Constants and Fields

       

        public static string DatabasePrefix { get; set; }

        private static readonly Dictionary<string, MongoMapperConfigurationElement> ConfigByObject = new Dictionary<string, MongoMapperConfigurationElement>();

        private static readonly Dictionary<string, MongoClientSettings> SettingsByObject = new Dictionary<string, MongoClientSettings>();

        private static readonly Dictionary<string, string> UrlByObject = new Dictionary<string, string>();


        private static readonly Object LockObject = new Object();

        private static readonly Object LockSettingsObject = new Object();

        #endregion



        #region Public Properties

        public static TextWriter Out { get; set; }

        #endregion

        #region Properties

        internal static int? ActiveServers { get; set; }

        internal static bool IsReplicaSet { get; set; }

        #endregion

        #region MultiConfiguration

        public enum ConfigurationKeyModeEnum
        {
            None,
            Fixed,
            Host,
            Path
        }

        private static string GlobalConfigurationKey { get; set; }

        private static readonly ConfigurationKeyModeEnum ConfigurationKeyMode = GetConfigurationKeyModeEnum();
        private static readonly int ConfigurationKeyPathIndex = GetConfigurationKeyPathIndex();

        private static ConfigurationKeyModeEnum GetConfigurationKeyModeEnum()
        {
            try
            {
                return (ConfigurationKeyModeEnum)Enum.Parse(typeof(ConfigurationKeyModeEnum), ConfigurationManager.AppSettings["MongoMapperConfigurationKeyMode"]);
            }
            catch (Exception)
            {
                return ConfigurationKeyModeEnum.None;
            }
        }

        private static int GetConfigurationKeyPathIndex()
        {
            try
            {
                return int.Parse(ConfigurationManager.AppSettings["MongoMapperConfigurationKeyPathIndex"]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static void SetGlobalCompanyKey(string CompanyKey)
        {
            GlobalConfigurationKey = CompanyKey;
        }


        public static string GetConfigurationKey()
        {
            if (!String.IsNullOrEmpty(GlobalConfigurationKey)) return GlobalConfigurationKey;

            if (HttpContext.Current == null)
            {
                return GetConfigurationKey(null);
            }
            Uri urlRequest = HttpContext.Current.Request.Url;
            return GetConfigurationKey(urlRequest);
        }

        private static string GetConfigurationKey(Uri UrlRequest)
        {
            if (ConfigurationKeyMode == ConfigurationKeyModeEnum.None)
            {
                return String.Empty;
            }
            if (ConfigurationKeyMode == ConfigurationKeyModeEnum.Fixed)
            {
                return ConfigurationManager.AppSettings["MongoMapperConfigurationKey"].ToUpper();
            }
            if (UrlRequest != null && ConfigurationKeyMode == ConfigurationKeyModeEnum.Host)
            {
                return UrlRequest.Host.Split('.')[ConfigurationKeyPathIndex].ToUpper();
            }
            if (UrlRequest != null && ConfigurationKeyMode == ConfigurationKeyModeEnum.Path)
            {
                return UrlRequest.PathAndQuery.Split('/')[ConfigurationKeyPathIndex + 1].ToUpper();
            }

            throw new Exception("Inconsistent ConfigurationKey Setup");
        }


        public static IMongoMapperConfiguration Config
        {
            get { return MongoMapperConfiguration.GetConfig(GetConfigurationKey()); }
        }

        #endregion

        #region Public Methods

        private static string _urlString = String.Empty;

        public static string DataBaseName(string ObjName)
        {
            if (CustomContext.Config != null)
            {
                return CustomContext.Config.Database;
            }

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

            MongoMapperConfigurationElement cfg = FindByObjName(ObjName);

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

        private static MongoMapperConfigurationElement FindByObjName(string ObjName)
        {
            return Config.CustomCollectionConfig.FirstOrDefault(C => C.Name == ObjName);            
        }

        #endregion
    }
}