using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoTTLIndex : Attribute
    {
        #region Constants and Fields

        public string IndexField;
        public int Seconds;

        #endregion
    }
}