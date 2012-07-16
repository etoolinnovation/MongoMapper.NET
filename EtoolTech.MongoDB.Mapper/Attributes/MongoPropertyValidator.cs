namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class MongoPropertyValidator : Attribute
    {
        #region Constants and Fields

        public string PropertyName;

        #endregion
    }
}