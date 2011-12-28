namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Xml.Serialization;

    using global::MongoDB.Bson.Serialization.Attributes;

    public interface IMongoMapperIdeable
    {
        [BsonId(IdGenerator = typeof(MongoMapperIdGenerator))]
        [XmlIgnore]
        long MongoMapper_Id { get; set; }
    }
}