using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoCustomDiscriminatorType : Attribute
    {
        public Type Type;
    }
}