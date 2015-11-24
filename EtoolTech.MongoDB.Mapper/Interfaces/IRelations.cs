using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Attributes;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IRelations<T>
    {
        #region Public Methods

        void EnsureDownRelations(object Sender, Type ClassType, IFinder Finder);

        void EnsureUpRelations(object Sender, Type ClassType, IFinder Finder);

        List<MongoRelation> GetDownRelations(Type T);

        List<T> GetRelation<T>(object Sender, string Relation, Type ClassType, IFinder Finder);

        List<MongoRelation> GetUpRelations(Type T);

        #endregion
    }
}