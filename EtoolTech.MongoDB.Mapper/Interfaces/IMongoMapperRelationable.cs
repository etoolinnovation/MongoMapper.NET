using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperRelationable
    {
        List<T> GetRelation<T>(string relation);

        void EnsureUpRelations();

        void EnsureDownRelations();
    }
}