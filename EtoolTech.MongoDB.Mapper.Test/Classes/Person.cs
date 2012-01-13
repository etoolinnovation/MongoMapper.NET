using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace EtoolTech.MongoDB.Mapper.Test.Classes
{
    [MongoKey(KeyFields = "")]
    [MongoIndex(IndexFields = "ID,Country")]
    [MongoIndex(IndexFields =  "Name")]
    public class Person : MongoMapper
    {
        public Person()
        {
            Childs = new List<Child>();
        }
                
        public string Name { get; set; }
        public int Age { get; set;}
        public DateTime BirthDate { get; set;}
        public bool Married { get; set;}
        public decimal BankBalance { get; set;}
        
        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }
             
        public List<Child> Childs { get; set;}
    }
}