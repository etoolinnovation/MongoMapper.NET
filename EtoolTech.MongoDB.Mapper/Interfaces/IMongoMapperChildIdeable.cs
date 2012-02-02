using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperChildIdeable
    {        
        [XmlIgnore]
        long _id { get; set; }
    }
}