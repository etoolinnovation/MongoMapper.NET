using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IMongoMapperCollection<T> : IEnumerable<T>
    {
        MongoCursor<T> Cursor { get; }
        long Total { get; }
        long Count { get; }
        MongoCursor<T> Find(IMongoQuery Query);
        MongoCursor<T> Find(Expression<Func<T, object>> Field, object Value);
        MongoCursor<T> Find();
        List<T> ToList();
        T First();
        T Last();
        IEnumerator<T> GetEnumerator();
    }
}