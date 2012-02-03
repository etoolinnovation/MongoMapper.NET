using System;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MongoChildCollection<T> : Attribute
    {        		
    }
}