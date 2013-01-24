using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoIndex : Attribute
    {
        #region Constants and Fields

        public string IndexFields;

        #endregion
    }
}