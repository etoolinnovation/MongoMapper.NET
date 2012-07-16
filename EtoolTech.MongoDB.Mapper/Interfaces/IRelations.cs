namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IRelations
    {
        #region Public Methods

        void EnsureDownRelations(object sender, Type classType, IFinder finder);

        void EnsureUpRelations(object sender, Type classType, IFinder finder);

        List<string> GetDownRelations(Type t);

        List<T> GetRelation<T>(object sender, string relation, Type classType, IFinder finder);

        List<string> GetUpRelations(Type t);

        #endregion
    }
}