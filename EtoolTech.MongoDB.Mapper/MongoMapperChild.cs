namespace EtoolTech.MongoDB.Mapper
{
    using System;

    using EtoolTech.MongoDB.Mapper.Interfaces;

    [Serializable]
    public abstract class MongoMapperChild : IMongoMapperChildIdeable
    {
        #region Public Properties

        public long _id { get; set; }

        #endregion
    }
}