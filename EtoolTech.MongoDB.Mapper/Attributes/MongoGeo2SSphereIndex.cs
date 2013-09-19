using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoGeo2DSphereIndex : Attribute
    {
        #region Constants and Fields

        public string IndexField;

        #endregion
    }
}