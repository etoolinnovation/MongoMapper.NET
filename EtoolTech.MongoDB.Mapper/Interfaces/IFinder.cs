using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IFinder
    {
        #region Public Methods

        MongoCursor<T> AllAsCursor<T>();

        List<T> AllAsList<T>();

        MongoCursor<T> FindAsCursor<T>(IMongoQuery Query);

        MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> Exp);

        List<T> FindAsList<T>(IMongoQuery Query);

        List<T> FindAsList<T>(Expression<Func<T, object>> Exp);

        BsonDocument FindBsonDocumentById<T>(long Id);

        T FindById<T>(long Id);

        object FindById(Type Type, long Id);

        T FindByKey<T>(params object[] Values);

        long FindIdByKey<T>(Dictionary<string, object> KeyValues);

        long FindIdByKey(Type T, Dictionary<string, object> KeyValues);

        T FindObjectByKey<T>(Dictionary<string, object> KeyValues);

        #endregion
    }
}