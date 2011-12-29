namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using global::MongoDB.Bson;

    public interface IMongoMapperBytesOriginalObject
    {
        string StringOriginalObject { get; set; }
    }

    public interface IMongoMapperOriginable : IMongoMapperBytesOriginalObject
    {
        T GetOriginalObject<T>();

        T GetOriginalT<T>();

        BsonDocument GetOriginalDocument();
    }
}