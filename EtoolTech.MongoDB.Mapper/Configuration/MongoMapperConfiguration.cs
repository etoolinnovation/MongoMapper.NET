namespace EtoolTech.MongoDB.Mapper.Configuration
{
    using System.Configuration;

    public class MongoMapperConfiguration : ConfigurationSection
    {
        #region Constants and Fields

        private const string ConfigSectionName = "MongoMapperConfig";

        #endregion

        #region Public Properties

        [ConfigurationProperty("CollectionConfig")]
        public CollectionConfig CollectionConfig
        {
            get
            {
                return (CollectionConfig)this["CollectionConfig"] ?? new CollectionConfig();
            }
        }

        [ConfigurationProperty("Context")]
        public Context Context
        {
            get
            {
                return (Context)this["Context"] ?? new Context();
            }
        }

        [ConfigurationProperty("Database")]
        public Database Database
        {
            get
            {
                return (Database)this["Database"] ?? new Database();
            }
        }

        [ConfigurationProperty("Server")]
        public Server Server
        {
            get
            {
                return (Server)this["Server"] ?? new Server();
            }
        }

        #endregion

        #region Public Methods

        public static MongoMapperConfiguration GetConfig()
        {
            return (MongoMapperConfiguration)ConfigurationManager.GetSection(ConfigSectionName)
                   ?? new MongoMapperConfiguration();
        }

        #endregion
    }

    public class Server : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("BalancedReading", IsKey = false, IsRequired = false)]
        public bool BalancedReading
        {
            get
            {
                return bool.Parse(this["BalancedReading"].ToString());
            }
        }

        [ConfigurationProperty("Host", IsKey = true, IsRequired = true)]
        public string Host
        {
            get
            {
                return this["Host"] as string;
            }
        }

        [ConfigurationProperty("MinReplicaServersToWrite", IsKey = false, IsRequired = false)]
        public int MinReplicaServersToWrite
        {
            get
            {
                return int.Parse(this["MinReplicaServersToWrite"].ToString());
            }
        }

        [ConfigurationProperty("PoolSize", IsKey = false, IsRequired = true)]
        public int PoolSize
        {
            get
            {
                return int.Parse(this["PoolSize"].ToString());
            }
        }

        [ConfigurationProperty("ReplicaSetName", IsKey = false, IsRequired = false)]
        public string ReplicaSetName
        {
            get
            {
                return this["ReplicaSetName"].ToString();
            }
        }

        [ConfigurationProperty("WaitQueueTimeout", IsKey = false, IsRequired = true)]
        public int WaitQueueTimeout
        {
            get
            {
                return int.Parse(this["WaitQueueTimeout"].ToString());
            }
        }

        #endregion
    }

    public class Database : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("Name", IsKey = false, IsRequired = true)]
        public string Name
        {
            get
            {
                return this["Name"] as string;
            }
        }

        [ConfigurationProperty("Password", IsKey = false, IsRequired = false)]
        public string Password
        {
            get
            {
                return this["Password"] as string;
            }
        }

        [ConfigurationProperty("User", IsKey = false, IsRequired = false)]
        public string User
        {
            get
            {
                return this["User"] as string;
            }
        }

        #endregion
    }

    public class Context : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("EnableOriginalObject", IsKey = false, IsRequired = false)]
        public bool EnableOriginalObject
        {
            get
            {
                return bool.Parse(this["EnableOriginalObject"].ToString());
            }
        }

        [ConfigurationProperty("ExceptionOnDuplicateKey", IsKey = false, IsRequired = false)]
        public bool ExceptionOnDuplicateKey
        {
            get
            {
                return bool.Parse(this["ExceptionOnDuplicateKey"].ToString());
            }
        }

        [ConfigurationProperty("FSync", IsKey = false, IsRequired = false)]
        public bool FSync
        {
            get
            {
                return bool.Parse(this["FSync"].ToString());
            }
        }

        [ConfigurationProperty("Generated", IsKey = false, IsRequired = false)]
        public bool Generated
        {
            get
            {
                return bool.Parse(this["Generated"].ToString());
            }
        }

        [ConfigurationProperty("MaxDocumentSize", IsKey = false, IsRequired = true)]
        public int MaxDocumentSize
        {
            get
            {
                return int.Parse(this["MaxDocumentSize"].ToString());
            }
        }

        [ConfigurationProperty("SafeMode", IsKey = false, IsRequired = false)]
        public bool SafeMode
        {
            get
            {
                return bool.Parse(this["SafeMode"].ToString());
            }
        }

        [ConfigurationProperty("UseChidlsIncrementalId", IsKey = false, IsRequired = false)]
        public bool UseChidlsIncrementalId
        {
            get
            {
                return bool.Parse(this["UseChidlsIncrementalId"].ToString());
            }
        }

        [ConfigurationProperty("UseIncrementalId", IsKey = false, IsRequired = false)]
        public bool UseIncrementalId
        {
            get
            {
                return bool.Parse(this["UseIncrementalId"].ToString());
            }
        }

        #endregion
    }

    public class CollectionConfig : ConfigurationElementCollection
    {
        #region Public Properties

        [ConfigurationProperty("CollectionConfig", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CollectionElement), AddItemName = "add", ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public CollectionElement Collections
        {
            get
            {
                return (CollectionElement)base["CollectionConfig"];
            }
        }

        #endregion

        #region Methods

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionElement)element).Name;
        }

        #endregion
    }

    public class CollectionElement : ConfigurationElement
    {
        #region Public Properties

        [ConfigurationProperty("Context", IsRequired = false)]
        public Context Context
        {
            get
            {
                return this["Context"] as Context;
            }
        }

        [ConfigurationProperty("Database", IsRequired = false)]
        public Database Database
        {
            get
            {
                return this["Database"] as Database;
            }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["Name"] as string;
            }
        }

        [ConfigurationProperty("Server", IsRequired = false)]
        public Server Server
        {
            get
            {
                return this["Server"] as Server;
            }
        }

        #endregion
    }
}