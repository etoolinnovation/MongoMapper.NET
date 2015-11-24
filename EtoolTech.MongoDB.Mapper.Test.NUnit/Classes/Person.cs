using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [Serializable]
    [MongoKey(KeyFields = "")]
    [MongoIndex(IndexFields = "ID,Country")]
    [MongoIndex(IndexFields = "Name")]
    [MongoMapperIdIncrementable(IncremenalId = true, ChildsIncremenalId = true)]
    [MongoRelation("Country", "Country", "Code", true)]
    public class Person : MongoMapper<Person>
    {
        #region Constructors and Destructors

        public Person()
        {
            Childs = new List<Child>();
        }

        #endregion

        #region Public Properties

        [BsonElement("a")]
        [BsonDefaultValue(25)]
        [BsonIgnoreIfDefault]
        public int Age { get; set; }

        [BsonElement("bb")]
        public decimal BankBalance { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("bd")]
        public DateTime BirthDate { get; set; }

        [MongoChildCollection]
        public List<Child> Childs { get; set; }
        
        [BsonElement("c")]
        public string Country { get; set; }

        [BsonElement("m")]
        [BsonDefaultValue(false)]
        [BsonIgnoreIfDefault]
        public bool Married { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        #endregion
    }
}