using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [MongoKey(KeyFields = "Id")]
    [MongoIndex(IndexFields = "ID,Country")]
    [MongoIndex(IndexFields =  "Name")]
    public class Person : MongoMapper
    {
        public Person()
        {
            Childs = new List<Child>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set;}
        public DateTime BirthDate { get; set;}
        public bool Married { get; set;}
        public decimal BankBalance { get; set;}
        
        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }
             
        public List<Child> Childs { get; set;}
    }

    public class Child
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }
    }

    [MongoKey(KeyFields = "Code")]
    public class Country: MongoMapper
    {        
        [MongoDownRelation(ObjectName = "Person", FieldName = "Country")]
        public string Code { get; set; }
        public string Name { get; set; }

        [MongoPropertyValidator(PropertyName="Code")]
        public void CodeIsUp(string CountryCode)
        {
            if (CountryCode != CountryCode.ToUpper())
                throw new Exception(String.Format("{0} must be {1}", CountryCode, CountryCode.ToUpper()));
        }

    }

   

}