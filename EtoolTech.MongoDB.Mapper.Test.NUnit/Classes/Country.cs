using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [Serializable]
    [MongoKey(KeyFields = "Code")]    
    [MongoCollectionName(Name="Paises")]
    [MongoGeo2DIndex(IndexField="Pos")]
    [MongoGeo2DSphereIndex(IndexField="Area")]
    public class Country : MongoMapper
    {
        #region Public Properties

        [MongoDownRelation(ObjectName = "Person", FieldName = "Country")]
        [BsonElement("c")]
        public string Code { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        [BsonElement("p")]
        public long[] Pos { get; set; }

        public GeoArea Area { get; set; }

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

    [Serializable]
    public class GeoArea
    {
        public string type { get; set; }
        public double[][][] coordinates { get; set; }
    }
}