using System.Collections.Generic;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public interface IMongoMapperCollection<T> : IEnumerable<T>
    {
        MongoCursor<T> Cursor { get; }
        long Total { get; }
        long Count { get; }
        MongoCursor<T> Find(IMongoQuery Query);
        MongoCursor<T> Find(string FieldName, object Value);
        MongoCursor<T> Find();
        List<T> ToList();
        T First();
        T Last();
        IEnumerator<T> GetEnumerator();
    }
}