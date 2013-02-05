using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperWriteable
    {
        #region Public Methods

        void Delete();

        void DeleteDocument();

        void InsertDocument();

        int Save();

        void ServerUpdate(UpdateBuilder Update, bool Refill = true);

        void UpdateDocument(long Id);

        #endregion
    }
}