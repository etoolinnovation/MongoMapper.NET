using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoGeo2DIndex : Attribute
    {
        #region Constants and Fields

        public string IndexField;

        #endregion
    }
}