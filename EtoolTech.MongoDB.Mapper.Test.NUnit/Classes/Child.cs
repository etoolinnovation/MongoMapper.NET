namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;

    using EtoolTech.MongoDB.Mapper.Attributes;

    [Serializable]
    public class Child : MongoMapperChild
    {
        #region Public Properties

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }

        [MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
        public string Country { get; set; }

        public int ID { get; set; }

        public string Name { get; set; }

        #endregion
    }
}