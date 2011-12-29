using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperIdeable
    {
        [BsonId(IdGenerator = typeof (MongoMapperIdGenerator))]
        [XmlIgnore]
        long MongoMapper_Id { get; set; }
    }
}