using System;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    [Serializable]
    public abstract class MongoMapperChild : IMongoMapperChildIdeable
    {
        #region Public Properties

        public long _id { get; set; }

        #endregion
    }
}