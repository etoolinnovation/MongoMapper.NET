using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IMongoMapperCollection<T> : IEnumerable<T>
    {
        IFindFluent<T, T> Cursor { get; }
        long Total { get; }
        long Count { get; }
        IFindFluent<T, T> Find(FilterDefinition<T> Query);
        IFindFluent<T, T> Find(string JsonQuery);
        IFindFluent<T, T> Find(BsonDocument DocumentQuery);
        IFindFluent<T, T> Find(Expression<Func<T, object>> Field, object Value);
        IFindFluent<T, T> Find();
        //IFindFluent<T, T> IncludeFields(params string[] Fields);
        //IFindFluent<T, T> ExcludeFields(params string[] Fields);
        List<T> ToList();
        T First();
        T Last();        
    }
}