using System.Collections.Generic;
using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface IMongoMapperConfiguration
    {
        IContext Context { get; }
       
        IDatabase Database { get; }
        
        IServer Server { get; }

        List<ICollectionElement> CustomCollectionConfig { get; }

        CollectionConfig CollectionConfig { get; }
    }
}