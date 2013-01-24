using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoKey : Attribute
    {
        #region Constants and Fields

        public string KeyFields;

        #endregion
    }
}