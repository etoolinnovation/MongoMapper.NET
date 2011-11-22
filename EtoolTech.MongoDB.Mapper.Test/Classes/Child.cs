using System;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.Classes
{
    public class Child
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }
    }
}