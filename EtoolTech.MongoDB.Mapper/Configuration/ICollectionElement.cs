using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface ICollectionElement
    {

        MongoMapperConfigurationContext Context { get; set; }
        MongoMapperConfigurationDababase Database { get; set; }
        string Name { get; set; }
        MongoMapperConfigurationServer Server { get; set; }
    }
}