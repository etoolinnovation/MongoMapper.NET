using System.Collections.Generic;
using System.Configuration;

namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public interface ICollectionConfig : IList<ICollectionElement>
    {
        ICollectionElement Collections { get; }
    }
}