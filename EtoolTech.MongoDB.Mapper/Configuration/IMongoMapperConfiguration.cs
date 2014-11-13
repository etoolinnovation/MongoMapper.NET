using System.Collections.Generic;
using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface IMongoMapperConfiguration
    {
        string Key { get; set; }
        MongoMapperConfigurationContext Context { get; set; }
        MongoMapperConfigurationDababase Database { get; set; }
        MongoMapperConfigurationServer Server { get; set; }
        List<MongoMapperConfigurationElement> CustomCollectionConfig { get; set; }
        
    }
}