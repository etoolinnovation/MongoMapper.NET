using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class MongoMapperConfiguration : ConfigurationSection
    {
        private const string ConfigSectionName = "MongoMapperConfig";

        public static MongoMapperConfiguration GetConfig()
        {
            return (MongoMapperConfiguration) ConfigurationManager.GetSection(ConfigSectionName) ??
                   new MongoMapperConfiguration();
        }

        [ConfigurationProperty("Server")]
        public Server Server
        {
            get { return (Server) this["Server"] ?? new Server(); }
        }

        [ConfigurationProperty("Database")]
        public Database Database
        {
            get { return (Database)this["Database"] ?? new Database(); }
        }
    }
}

public class Server : ConfigurationElement
{
    [ConfigurationProperty("Host", IsKey = true, IsRequired = true)]
    public string Host
    {
        get { return this["Host"] as string; }
    }

    [ConfigurationProperty("Port", IsKey = false, IsRequired = true)]
    public int Port
    {
        get { return  int.Parse(this["Port"].ToString()); }
    }
  

    [ConfigurationProperty("PoolSize", IsKey = false, IsRequired = true)]
    public int PoolSize
    {
        get { return int.Parse(this["PoolSize"].ToString()); }
    }
}

public class Database : ConfigurationElement
{
   
    [ConfigurationProperty("Name", IsKey = false, IsRequired = true)]
    public string Name
    {
        get { return this["Name"] as string; }
    }

    [ConfigurationProperty("User", IsKey = false, IsRequired = true)]
    public string User
    {
        get { return this["User"] as string; }
    }

    [ConfigurationProperty("Password", IsKey = false, IsRequired = true)]
    public string Password
    {
        get { return this["Password"] as string; }
    }

}
