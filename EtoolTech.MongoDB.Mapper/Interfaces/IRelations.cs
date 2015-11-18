using System;
using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IRelations<T>
    {
        #region Public Methods

        void EnsureDownRelations(object Sender, Type ClassType, IFinder Finder);

        void EnsureUpRelations(object Sender, Type ClassType, IFinder Finder);

        List<string> GetDownRelations(Type T);

        List<T> GetRelation<T>(object Sender, string Relation, Type ClassType, IFinder Finder);

        List<string> GetUpRelations(Type T);

        #endregion
    }
}