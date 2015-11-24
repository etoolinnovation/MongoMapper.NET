using System;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [Serializable]
    [MongoRelation("Country","Country","Code", UpRelation:true)]
    public class Child : MongoMapperChild
    {
        #region Public Properties

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
        
        public string Country { get; set; }

        public int ID { get; set; }

        public string Name { get; set; }

        #endregion
    }
}