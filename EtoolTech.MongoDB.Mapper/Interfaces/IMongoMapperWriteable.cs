namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using global::MongoDB.Driver.Builders;

    public interface IMongoMapperWriteable
    {
        void Save<T>();

        void ServerUpdate<T>(UpdateBuilder update, bool refill = true);

        void UpdateDocument(long id);

        void InsertDocument();

        void Delete<T>();

        void DeleteDocument<T>();
    }
}