using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MongoPropertyValidator : Attribute
    {
        public string PropertyName;
    }
}