using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MongoCollectionName : Attribute
    {
        public string Name;
    }
}