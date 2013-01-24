namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;

    using EtoolTech.MongoDB.Mapper.Attributes;

    [Serializable]
    [MongoKey(KeyFields = "Code")]
    [MongoGeo2DIndex(IndexField = "Pos")]
    public class Country : MongoMapper
    {
        #region Public Properties

        [MongoDownRelation(ObjectName = "Person", FieldName = "Country")]
        public string Code { get; set; }

        public string Name { get; set; }

        public long[] Pos { get; set; }

        #endregion

        #region Public Methods

        [MongoPropertyValidator(PropertyName = "Code")]
        public void CodeIsUp(string CountryCode)
        {
            if (CountryCode != CountryCode.ToUpper())
            {
                throw new Exception(String.Format("{0} must be {1}", CountryCode, CountryCode.ToUpper()));
            }
        }

        #endregion
    }
}