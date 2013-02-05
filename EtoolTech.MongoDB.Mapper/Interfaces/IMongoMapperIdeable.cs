using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperIdeable
    {
        #region Public Properties

        [BsonId(IdGenerator = typeof (MongoMapperIdGenerator))]
        [XmlIgnore]
        long m_id { get; set; }

        #endregion
    }
}