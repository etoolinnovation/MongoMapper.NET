using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperCollection<T> : IMongoMapperCollection<T>
    {
        public MongoCursor<T> Cursor { get; private set; }

        public static MongoMapperCollection<T> Instance {get{return new MongoMapperCollection<T>();}} 

        public MongoCursor<T> Find(IMongoQuery Query)
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(Query);
            return Cursor;
        }

        public MongoCursor<T> Find(string JsonQuery)
        {
            var document = ObjectSerializer.JsonStringToBsonDocument(JsonQuery);
            var query = new QueryDocument(document);
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(query);
            return Cursor;
        }

        public MongoCursor<T> Find(Expression<Func<T, object>> Field, object Value)
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(MongoQuery<T>.Eq(Field, Value));
            return Cursor;
        }

        public MongoCursor<T> Find(string FieldName, object Value)
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(MongoQuery.Eq(typeof(T).Name, FieldName, Value));
            return Cursor;
        }

        public MongoCursor<T> Find()
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAllAs<T>();          
            return Cursor;
        }
   

        public T Pop(IMongoQuery CustomQuery, IMongoSortBy SortBy)
        {
            var col = CollectionsManager.GetCollection(typeof(T).Name);

            var args = new FindAndRemoveArgs {SortBy = SortBy, Query = CustomQuery};

            var result = col.FindAndRemove(args);

            if (result.Ok)
            {
                return result.GetModifiedDocumentAs<T>();
            }

            return default(T);

        }

        public T Pop()
        {
            return Pop(Query.Null, SortBy.Ascending("$natural"));
        }

        public long Total
        {
            get { return Cursor.Count(); }
        }

        public long Count 
        {
            get { return Cursor.Size(); }
        }

        public List<T> ToList()
        {
            return Cursor.ToList();
        }

        public T First()
        {
            return Cursor.First();
        }

        public T Last()
        {
            return Cursor.Last();
        }

        public IEnumerator<T> GetEnumerator()
        {                        
            return Cursor.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

   
}
