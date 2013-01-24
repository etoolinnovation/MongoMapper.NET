using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperWriteable
    {
        #region Public Methods

        void Delete<T>();

        void DeleteDocument<T>();

        void InsertDocument();

        int Save<T>();

        void ServerUpdate<T>(UpdateBuilder Update, bool Refill = true);

        void UpdateDocument(long Id);

        #endregion
    }
}