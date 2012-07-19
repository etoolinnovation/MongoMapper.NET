namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using global::MongoDB.Driver.Builders;

    public interface IMongoMapperWriteable
    {
        #region Public Methods

        void Delete<T>();

        void DeleteDocument<T>();

        void InsertDocument();

        int Save<T>();

        void ServerUpdate<T>(UpdateBuilder update, bool refill = true);

        void UpdateDocument(long id);

        #endregion
    }
}