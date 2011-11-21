using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoCountField : Attribute
    {
        public string ClassName;
        public string FieldName;
    }
}