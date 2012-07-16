namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoCustomDiscriminatorType : Attribute
    {
        #region Constants and Fields

        public Type Type;

        #endregion
    }
}