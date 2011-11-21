using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface ITransaction
    {
        List<string> Collections { get; set; }
    }
}