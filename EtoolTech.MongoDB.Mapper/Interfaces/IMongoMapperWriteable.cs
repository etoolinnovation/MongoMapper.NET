using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperWriteable<T>
    {
        #region Public Methods

        void Delete();

        void DeleteDocument();

        void InsertDocument();

        int Save();

        void ServerUpdate(UpdateDefinition<T> Update, bool Refill = true);

        void UpdateDocument(long Id);

        #endregion
    }
}