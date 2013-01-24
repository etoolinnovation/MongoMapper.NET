using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MongoPropertyValidator : Attribute
    {
        #region Constants and Fields

        public string PropertyName;

        #endregion
    }
}