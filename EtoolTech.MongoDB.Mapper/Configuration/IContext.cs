using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface IContext
    {
      
        bool EnableOriginalObject { get; }

    
        bool ExceptionOnDuplicateKey { get; }

      
        bool Generated { get; }

     
        int MaxDocumentSize { get; }

      
        bool UseChidlsIncrementalId { get; }

    
        bool UseIncrementalId { get; }
    }
}