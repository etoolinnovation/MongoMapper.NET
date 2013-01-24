using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoCollectionName : Attribute
    {
        #region Constants and Fields

        public string Name;

        #endregion
    }
}