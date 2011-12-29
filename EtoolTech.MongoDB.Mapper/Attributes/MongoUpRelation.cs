using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoUpRelation : Attribute
    {
        public string FieldName;
        public string ObjectName;

        public string ParentFieldName;

        public string ParentPropertyName;
    }
}