namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using global::MongoDB.Bson;

    public interface IMongoMapperOriginable
    {
        byte[] BytesOriginalObject { get; set; }

        object GetOriginalValue<T>(string fieldName);

        T GetOriginalObject<T>();

        T GetOriginalT<T>();

        BsonDocument GetOriginalDocument();
    }
}