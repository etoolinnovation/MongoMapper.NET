using System.Collections.Generic;
using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface IMongoMapperConfiguration
    {
        MongoMapperConfigurationContext Context { get; set; }
        MongoMapperConfirgurationDababase Database { get; set; }
        MongoMapperConfigurationServer Server { get; set; }
        List<MongoMapperConfigurationElement> CustomCollectionConfig { get; set; }
        
    }
}