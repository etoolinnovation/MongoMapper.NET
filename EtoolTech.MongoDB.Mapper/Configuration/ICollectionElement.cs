using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface ICollectionElement
    {
        
        IContext Context { get; }
        IDatabase Database { get; }        
        string Name { get; }      
        IServer Server { get; }
    }
}