namespace EtoolTech.MongoDB.Mapper
{
    using System.Collections.Generic;

    using EtoolTech.MongoDB.Mapper.Interfaces;

    public class Transaction : MongoMapper, ITransaction
    {
        #region Public Properties

        public List<string> Collections { get; set; }

        #endregion
    }
}