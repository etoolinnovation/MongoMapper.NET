namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System.Collections.Generic;

    public interface IMongoMapperRelationable
    {
        List<T> GetRelation<T>(string relation);

        void EnsureUpRelations();

        void EnsureDownRelations();
    }
}