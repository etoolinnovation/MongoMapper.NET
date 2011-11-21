using System;
using System.Linq.Expressions;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoKey : Attribute
    {
        public string KeyFields;                
    }
}