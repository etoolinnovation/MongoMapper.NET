using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IFinder
    {
        T FindById<T>(long id);

        object FindById(Type type, long id);

        BsonDocument FindBsonDocumentById<T>(long id);

        T FindByKey<T>(params object[] values);

        long FindIdByKey<T>(Dictionary<string, object> keyValues);

        T FindObjectByKey<T>(Dictionary<string, object> keyValues);

        List<T> FindAsList<T>(IMongoQuery query);

        List<T> FindAsList<T>(Expression<Func<T, object>> exp);

        List<T> AllAsList<T>();

        MongoCursor<T> FindAsCursor<T>(IMongoQuery query);

        MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp);

        MongoCursor<T> AllAsCursor<T>();
    }
}