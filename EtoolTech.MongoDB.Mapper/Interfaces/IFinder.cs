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
        T FindById<T>(int id);
        BsonDocument FindBsonDocumentById<T>(int id);

        T FindByKey<T>(params object[] values);
        int FindIdByKey<T>(Dictionary<string, object> keyValues);
        T FindObjectByKey<T>(Dictionary<string, object> keyValues);
             
        List<T> FindAsList<T>(QueryComplete query);      
        List<T> FindAsList<T>(Expression<Func<T, object>> exp);
        List<T> AllAsList<T>();

        MongoCursor<T> FindAsCursor<T>(QueryComplete query);        
        MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp);
        MongoCursor<T> AllAsCursor<T>();
        
        QueryComplete GetEqQuery(Type type, string fieldName, object value);
        QueryComplete GetGtQuery(Type type, string fieldName, object value);
        QueryComplete GetGteQuery(Type type, string fieldName, object value);
        QueryComplete GetLtQuery(Type type, string fieldName, object value);
        QueryComplete GetLteQuery(Type type, string fieldName, object value);
        QueryComplete GetNeQuery(Type type, string fieldName, object value);
        QueryComplete GetRegEx(string fieldName, string expresion);

        
    }
}