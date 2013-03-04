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
    public class Person : MongoMapper
    {
        #region Constructors and Destructors

        public Person()
        {
            Childs = new List<Child>();
        }

        #endregion

        #region Public Properties

        [BsonElement("a")]
        public int Age { get; set; }

        [BsonElement("bb")]
        [BsonDefaultValue(0.00)]
        public decimal BankBalance { get; set; }

        public DateTime BirthDate { get; set; }

        [MongoChildCollection]
        public List<Child> Childs { get; set; }

        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
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