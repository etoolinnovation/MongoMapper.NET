using System.Collections.Generic;

using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public class Transaction : MongoMapper, ITransaction
    {
        public List<string> Collections { get; set; }
    }
}