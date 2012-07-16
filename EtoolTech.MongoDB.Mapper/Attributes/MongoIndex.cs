namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoIndex : Attribute
    {
        #region Constants and Fields

        public string IndexFields;

        #endregion
    }
}