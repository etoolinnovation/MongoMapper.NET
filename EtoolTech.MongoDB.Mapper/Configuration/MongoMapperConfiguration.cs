using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class MongoMapperConfiguration : ConfigurationSection
    {
        private const string ConfigSectionName = "MongoMapperConfig";

        [ConfigurationProperty("Server")]
        public Server Server
        {
            get
            {
                return (Server)this["Server"] ?? new Server();
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

        [ConfigurationProperty("Context")]
        public Context Context
        {
            get
            {
                return (Context)this["Context"] ?? new Context();
            }
        }

        [ConfigurationProperty("CollectionConfig")]
        public CollectionConfig CollectionConfig
        {
            get
            {
                return (CollectionConfig)this["CollectionConfig"] ?? new CollectionConfig();
            }
        }

        public static MongoMapperConfiguration GetConfig()
        {
            return (MongoMapperConfiguration)ConfigurationManager.GetSection(ConfigSectionName)
                   ?? new MongoMapperConfiguration();
        }
    }

    public class Server : ConfigurationElement
    {
        [ConfigurationProperty("Host", IsKey = true, IsRequired = true)]
        public string Host
        {
            get
            {
                return this["Host"] as string;
            }
        }

        [ConfigurationProperty("Port", IsKey = false, IsRequired = true)]
        public int Port
        {
            get
            {
                return int.Parse(this["Port"].ToString());
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

        [ConfigurationProperty("WaitQueueTimeout", IsKey = false, IsRequired = true)]
        public int WaitQueueTimeout
        {
            get
            {
                return int.Parse(this["WaitQueueTimeout"].ToString());
            }
        }
    }

    public class Database : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = false, IsRequired = true)]
        public string Name
        {
            get
            {
                return this["Name"] as string;
            }
        }

        [ConfigurationProperty("User", IsKey = false, IsRequired = true)]
        public string User
        {
            get
            {
                return this["User"] as string;
            }
        }

        [ConfigurationProperty("Password", IsKey = false, IsRequired = true)]
        public string Password
        {
            get
            {
                return this["Password"] as string;
            }
        }
    }

    public class Context : ConfigurationElement
    {
        [ConfigurationProperty("Generated", IsKey = false, IsRequired = true)]
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

        [ConfigurationProperty("SafeMode", IsKey = false, IsRequired = true)]
        public bool SafeMode
        {
            get
            {
                return bool.Parse(this["SafeMode"].ToString());
            }
        }

        [ConfigurationProperty("FSync", IsKey = false, IsRequired = true)]
        public bool FSync
        {
            get
            {
                return bool.Parse(this["FSync"].ToString());
            }
        }

        [ConfigurationProperty("ExceptionOnDuplicateKey", IsKey = false, IsRequired = true)]
        public bool ExceptionOnDuplicateKey
        {
            get
            {
                return bool.Parse(this["ExceptionOnDuplicateKey"].ToString());
            }
        }

        [ConfigurationProperty("EnableOriginalObject", IsKey = false, IsRequired = true)]
        public bool EnableOriginalObject
        {
            get
            {
                return bool.Parse(this["EnableOriginalObject"].ToString());
            }
        }

        [ConfigurationProperty("UseIncrementalId", IsKey = false, IsRequired = true)]
        public bool UseIncrementalId
        {
            get
            {
                return bool.Parse(this["UseIncrementalId"].ToString());
            }
        }
		
		
        [ConfigurationProperty("UseChidlsIncrementalId", IsKey = false, IsRequired = true)]
        public bool UseChidlsIncrementalId
        {
            get
            {
                return bool.Parse(this["UseChidlsIncrementalId"].ToString());
            }
        }
    }

    public class CollectionConfig : ConfigurationElementCollection
    {
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

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CollectionElement)element).Name;
        }
    }

    public class CollectionElement : ConfigurationElement
    {
        [System.Configuration.ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["Name"] as string;
            }
        }

        [System.Configuration.ConfigurationProperty("Database", IsRequired = false)]
        public Database Database
        {
            get
            {
                return this["Database"] as Database;
            }
        }

        [System.Configuration.ConfigurationProperty("Server", IsRequired = false)]
        public Server Server
        {
            get
            {
                return this["Server"] as Server;
            }
        }

        [System.Configuration.ConfigurationProperty("Context", IsRequired = false)]
        public Context Context
        {
            get
            {
                return this["Context"] as Context;
            }
        }
    }
}