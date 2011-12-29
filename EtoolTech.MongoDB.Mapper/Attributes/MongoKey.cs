using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoKey : Attribute
    {
        public string KeyFields;
    }
}