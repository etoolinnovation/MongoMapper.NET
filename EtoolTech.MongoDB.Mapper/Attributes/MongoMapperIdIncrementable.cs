using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoMapperIdIncrementable : Attribute
    {
        #region Public Properties

        public bool ChildsIncremenalId { get; set; }

        public bool IncremenalId { get; set; }

        #endregion
    }
}