using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public class Transaction : MongoMapper, ITransaction
    {
        #region ITransaction Members

        public List<string> Collections { get; set; }

        #endregion
    }
}