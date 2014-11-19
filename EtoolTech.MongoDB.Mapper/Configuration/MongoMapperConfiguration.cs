using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class MongoMapperConfiguration : ConfigurationSection
    {
        #region Constants and Fields

        private const string ConfigSectionName = "MongoMapperConfig";

        #endregion

        #region Public Properties

        [ConfigurationProperty("CollectionConfig")]
        public CollectionConfig CollectionConfig
        {
            get { return (CollectionConfig) this["CollectionConfig"] ?? new CollectionConfig(); }
        }

        [ConfigurationProperty("Context")]
        public Context Context
        {
            get { return (Context) this["Context"] ?? new Context(); }
        }

        [ConfigurationProperty("Database")]
        public Database Database
        {
            get { return (Database) this["Database"] ?? new Database(); }
        }

        [ConfigurationProperty("Server")]
        public Server Server
        {
            get { return (Server) this["Server"] ?? new Server(); }
        }

        #endregion

        #region Public Methods

        public static void SetDbConfig(IMongoMapperConfiguration Configuration)
        {
            string dbConfigKey = System.Configuration.ConfigurationManager.AppSettings["MongoMapperDbConfig"];

            if (!String.IsNullOrEmpty(dbConfigKey))
            {
                string[] values = dbConfigKey.Split('|');
                var client = new MongoClient(values[0]);
                var server = client.GetServer();
                var db = server.GetDatabase(values[1]);
                db.GetCollection<MongoMapperConfiguracionBase>(values[2]).Insert((MongoMapperConfiguracionBase) Configuration);
            }
            
        }

        public static IMongoMapperConfiguration GetConfig()
        {

            string dbConfigKey = System.Configuration.ConfigurationManager.AppSettings["MongoMapperDbConfig"];

            if (!String.IsNullOrEmpty(dbConfigKey))
            {
                string[] values = dbConfigKey.Split('|');
                var client = new MongoClient(values[0]);
                var server = client.GetServer();
                var db = server.GetDatabase(values[1]);
                var config = db.GetCollection<MongoMapperConfiguracionBase>(values[2]).FindOneAs<MongoMapperConfiguracionBase>(Query.EQ("Key", values[3]));

                return config;
            }
            else
            {

                var fileConfig = (MongoMapperConfiguration) ConfigurationManager.GetSection(ConfigSectionName);

                var config = new MongoMapperConfiguracionBase
                {
                    Context =
                        new MongoMapperConfigurationContext
                        {
                            EnableOriginalObject = fileConfig.Context.EnableOriginalObject,
                            ExceptionOnDuplicateKey = fileConfig.Context.ExceptionOnDuplicateKey,
                            Generated = fileConfig.Context.Generated,
                            MaxDocumentSize = fileConfig.Context.MaxDocumentSize,
                            UseChidlsIncrementalId = fileConfig.Context.UseChidlsIncrementalId,
                            UseIncrementalId = fileConfig.Context.UseIncrementalId
                        },
                    Database = new MongoMapperConfigurationDababase {Name = fileConfig.Database.Name},
                    Server = new MongoMapperConfigurationServer {Url = fileConfig.Server.Url},
                    CustomCollectionConfig = new List<MongoMapperConfigurationElement>()
                };

                foreach (CollectionElement element in fileConfig.CollectionConfig)
                {
                    var configElement = new MongoMapperConfigurationElement
                    {
                        Name = element.Name,
                        Context =
                            new MongoMapperConfigurationContext
                            {
                                EnableOriginalObject = element.Context.EnableOriginalObject,
                                ExceptionOnDuplicateKey = element.Context.ExceptionOnDuplicateKey,
                                Generated = element.Context.Generated,
                                MaxDocumentSize = element.Context.MaxDocumentSize,
                                UseChidlsIncrementalId = element.Context.UseChidlsIncrementalId,
                                UseIncrementalId = element.Context.UseIncrementalId
                            },
                        Database = new MongoMapperConfigurationDababase {Name = element.Database.Name},
                        Server = new MongoMapperConfigurationServer {Url = element.Server.Url}
                    };


                    config.CustomCollectionConfig.Add(configElement);

                }
                
                return config;
            }

        }

        #endregion
    }

    public class Server : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("Url", IsKey = true, IsRequired = true)]
        public string Url
        {
            get { return this["Url"] as string; }
        }

        #endregion
    }

    public class Database : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("Name", IsKey = false, IsRequired = true)]
        public string Name
        {
            get { return this["Name"] as string; }
        }

        #endregion
    }

    public class Context : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("EnableOriginalObject", IsKey = false, IsRequired = false)]
        public bool EnableOriginalObject
        {
            get { return bool.Parse(this["EnableOriginalObject"].ToString()); }
        }

        [ConfigurationProperty("ExceptionOnDuplicateKey", IsKey = false, IsRequired = false)]
        public bool ExceptionOnDuplicateKey
        {
            get { return bool.Parse(this["ExceptionOnDuplicateKey"].ToString()); }
        }


        [ConfigurationProperty("Generated", IsKey = false, IsRequired = false)]
        public bool Generated
        {
            get { return bool.Parse(this["Generated"].ToString()); }
        }

        [ConfigurationProperty("MaxDocumentSize", IsKey = false, IsRequired = true)]
        public int MaxDocumentSize
        {
            get { return int.Parse(this["MaxDocumentSize"].ToString()); }
        }

        [ConfigurationProperty("UseChidlsIncrementalId", IsKey = false, IsRequired = false)]
        public bool UseChidlsIncrementalId
        {
            get { return bool.Parse(this["UseChidlsIncrementalId"].ToString()); }
        }

        [ConfigurationProperty("UseIncrementalId", IsKey = false, IsRequired = false)]
        public bool UseIncrementalId
        {
            get { return bool.Parse(this["UseIncrementalId"].ToString()); }
        }

        #endregion
    }

    public class CollectionConfig : ConfigurationElementCollection
    {
        #region Public Properties

        [ConfigurationProperty("CollectionConfig", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (CollectionElement), AddItemName = "add", ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public CollectionElement Collections
        {
            get { return (CollectionElement) base["CollectionConfig"]; }
        }

        #endregion

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionElement) element).Name;
        }

        #endregion
    }

    public class CollectionElement : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("Context", IsRequired = false)]
        public Context Context
        {
            get { return this["Context"] as Context; }
        }

        [ConfigurationProperty("Database", IsRequired = false)]
        public Database Database
        {
            get { return this["Database"] as Database; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return this["Name"] as string; }
        }

        [ConfigurationProperty("Server", IsRequired = false)]
        public Server Server
        {
            get { return this["Server"] as Server; }
        }

        #endregion
    }
}