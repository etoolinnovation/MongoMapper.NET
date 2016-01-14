using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperRelationable<T>
    {
        #region Public Methods

        void EnsureDownRelations();

        void EnsureUpRelations();

        #endregion
    }
}