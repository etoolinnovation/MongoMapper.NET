using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class MongoMapperConfiguracionBase: IMongoMapperConfiguration
    {
        public object _id { get; set; }
        public string Key { get; set; }
        public MongoMapperConfigurationContext Context { get; set; }
        public MongoMapperConfigurationDababase Database { get; set; }
        public MongoMapperConfigurationServer Server { get; set; }
        public List<MongoMapperConfigurationElement> CustomCollectionConfig { get; set; }
    }

    public class MongoMapperConfigurationContext : IContext
    {
        public bool EnableOriginalObject { get; set; }
        public bool ExceptionOnDuplicateKey { get; set; }
        public bool Generated { get; set; }
        public int MaxDocumentSize { get; set; }
        public bool UseChidlsIncrementalId { get; set; }
        public bool UseIncrementalId { get; set; }
    }


    public class MongoMapperConfigurationDababase : IDatabase
    {
        public string Name { get; set; }
    }

    public class MongoMapperConfigurationServer: IServer
    {
        public string Url { get; set; }
    }

   
    public class MongoMapperConfigurationElement : ICollectionElement
    {
        public MongoMapperConfigurationContext Context { get; set; }
        public MongoMapperConfigurationDababase Database { get; set; }
        public string Name { get;  set; }
        public MongoMapperConfigurationServer Server { get; set; }
    }
}
