using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IMongoMapperCollection<T> : IEnumerable<T>
    {
        IFindFluent<T, T> Cursor { get; }
        long Total { get; }
        long Count { get; }
        IFindFluent<T, T> Find(FilterDefinition<T> Query);
        IFindFluent<T, T> Find(Expression<Func<T, object>> Field, object Value);
        IFindFluent<T, T> Find();
        List<T> ToList();
        T First();
        T Last();        
    }
}