using System;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
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