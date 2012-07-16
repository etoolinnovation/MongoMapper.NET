namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class MongoMapperIdIncrementable : Attribute
    {
        #region Public Properties

        public bool ChildsIncremenalId { get; set; }

        public bool IncremenalId { get; set; }

        #endregion
    }
}