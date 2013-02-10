using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperCollection<T> : IEnumerable<T> where T : MongoMapper
    {
        public MongoCursor<T> Cursor { get; private set; }

        public static MongoMapperCollection<T> Instance {get{return new MongoMapperCollection<T>();}} 

        public MongoCursor<T> Find(IMongoQuery Query)
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(Query);
            return Cursor;
        }

        public MongoCursor<T> Find(string FieldName, object Value)
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(MongoQuery.Eq(FieldName,Value));
            return Cursor;
        }

        public MongoCursor<T> Find()
        {
            Cursor = CollectionsManager.GetCollection(typeof(T).Name).FindAllAs<T>();          
            return Cursor;
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
            return new MongoCursorEnumerator<T>(Cursor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

   
}
