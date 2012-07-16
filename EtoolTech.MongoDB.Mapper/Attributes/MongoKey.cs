namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class MongoKey : Attribute
    {
        #region Constants and Fields

        public string KeyFields;

        #endregion
    }
}