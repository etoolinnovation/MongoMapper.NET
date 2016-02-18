using System.Threading.Tasks;
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

        Task<int> SaveAsync();

        void ServerUpdate(UpdateDefinition<T> Update, bool Refill = true);

        Task ServerUpdateAsync(UpdateDefinition<T> Update, bool Refill = true);

        void ServerSetValue(string FieldName, object Value, bool Refill = true);

        Task ServerSetValueAsync(string FieldName, object Value, bool Refill = true);

        void UpdateDocument(long Id);

        #endregion
    }
}