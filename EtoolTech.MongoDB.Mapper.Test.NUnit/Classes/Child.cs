using System;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using EtoolTech.MongoDB.Mapper;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Child: MongoMapperChild
    {        
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }
    }
}