namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver;

    public interface IFinder
    {
        #region Public Methods

        MongoCursor<T> AllAsCursor<T>();

        List<T> AllAsList<T>();

        MongoCursor<T> FindAsCursor<T>(IMongoQuery query);

        MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp);

        List<T> FindAsList<T>(IMongoQuery query);

        List<T> FindAsList<T>(Expression<Func<T, object>> exp);

        BsonDocument FindBsonDocumentById<T>(long id);

        T FindById<T>(long id);

        object FindById(Type type, long id);

        T FindByKey<T>(params object[] values);

        long FindIdByKey<T>(Dictionary<string, object> keyValues);

        T FindObjectByKey<T>(Dictionary<string, object> keyValues);

        #endregion
    }
}