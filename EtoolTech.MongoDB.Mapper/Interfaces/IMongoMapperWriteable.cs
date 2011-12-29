namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperWriteable
    {
        void Save<T>();

        void UpdateDocument(long id);

        void InsertDocument();

        void Delete<T>();

        void DeleteDocument<T>();
    }
}