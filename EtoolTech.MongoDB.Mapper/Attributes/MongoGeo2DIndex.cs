namespace EtoolTech.MongoDB.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoGeo2DIndex : Attribute
    {
        #region Constants and Fields

        public string IndexField;

        #endregion
    }
}