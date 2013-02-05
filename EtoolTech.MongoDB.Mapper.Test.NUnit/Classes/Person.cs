using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;

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

        public int Age { get; set; }

        public decimal BankBalance { get; set; }

        public DateTime BirthDate { get; set; }

        [MongoChildCollection]
        public List<Child> Childs { get; set; }

        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }

        public bool Married { get; set; }

        public string Name { get; set; }

        #endregion
    }
}