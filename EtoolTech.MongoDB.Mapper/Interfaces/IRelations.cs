using System;
using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IRelations
    {
        List<string> GetUpRelations(Type t);
        List<string> GetDownRelations(Type t);
        List<T> GetRelation<T>(object sender, string relation, Type classType, IFinder finder);
        void EnsureUpRelations(object sender, Type classType, IFinder finder);
        void EnsureDownRelations(object sender, Type classType, IFinder finder);
    }
}