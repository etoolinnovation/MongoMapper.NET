using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoUpRelation : Attribute
    {
        #region Constants and Fields

        public string FieldName;

        public string ObjectName;

        public string ParentFieldName;

        public string ParentPropertyName;

        #endregion
    }
}