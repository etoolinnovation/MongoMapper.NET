using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IFinder
    {
        T FindById<T>(Guid id);
        BsonDocument FindBsonDocumentById<T>(Guid id);
        T FindByKey<T>(params object[] values);
        Guid FindGuidByKey<T>(Dictionary<string, object> keyValues);
        T FindObjectByKey<T>(Dictionary<string, object> keyValues);

        List<T> FindAnd<T>(List<IFindOptions> findOptions);
        List<T> FindOr<T>(List<IFindOptions> findOptions);
        List<T> Find<T>(string field, object value, FindCondition findCondition = FindCondition.Equal);
        List<T> Find<T>(IFindOptions findOptions);
        List<T> Find<T>(Expression<Func<T, object>> exp);
        QueryComplete GetQuery(FindCondition findCondition, object value, string field);
        QueryComplete GetEqQuery(Type type, string fieldName, object value);
        QueryComplete GetGtQuery(Type type, string fieldName, object value);
        QueryComplete GetGteQuery(Type type, string fieldName, object value);
        QueryComplete GetLtQuery(Type type, string fieldName, object value);
        QueryComplete GetLteQuery(Type type, string fieldName, object value);
        QueryComplete GetNeQuery(Type type, string fieldName, object value);
        QueryComplete GetRegEx(string fieldName, string expresion);
        List<T> All<T>();
    }
}