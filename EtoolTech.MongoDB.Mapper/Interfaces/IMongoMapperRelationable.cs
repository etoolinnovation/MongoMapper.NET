namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Collections.Generic;

    public interface IMongoMapperRelationable
    {
        #region Public Methods

        void EnsureDownRelations();

        void EnsureUpRelations();

        List<T> GetRelation<T>(string relation);

        #endregion
    }
}