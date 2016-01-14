using System;
using System.Diagnostics;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class Writer : IWriter
    {
        internal static IWriter Instance
        {
            get { return new Writer(); }
        }

        #region IWriter Members

        public bool Insert<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Insert, Type, Document);
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            CollectionsManager.GetCollection<T>(Name).InsertOneAsync(Document).GetAwaiter().GetResult();

            return true;

        }

        public bool Update<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Update, Type, Document);              
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            var mongoMapperIdeable = Document as IMongoMapperIdeable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            Debug.Assert(mongoMapperIdeable != null, "mongoMapperIdeable != null");

            var result = CollectionsManager.GetCollection<T>(Name).ReplaceOneAsync(
                Builders<T>.Filter.Eq("_id", mongoMapperIdeable.m_id),
                Document,
                new UpdateOptions {IsUpsert = true}
                ).GetAwaiter().GetResult();


            return result.ModifiedCount > 0;
        }

        public bool Delete<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Delete, Type, Document);
                return true;
            }

            var mongoMapperIdeable = Document as IMongoMapperIdeable;

            Debug.Assert(mongoMapperIdeable != null, "mongoMapperIdeable != null");

            if (mongoMapperIdeable.m_id == default(long))
            {
                mongoMapperIdeable.m_id = Finder.Instance.FindIdByKey<T>(Type,
                                                                            MongoMapperHelper.GetPrimaryKey(Type).
                                                                                ToDictionary(
                                                                                    KeyField => KeyField,
                                                                                    KeyField =>
                                                                                    ReflectionUtility.
                                                                                        GetPropertyValue(
                                                                                            this, KeyField)));
            }

            var query = Builders<T>.Filter.Eq("_id", mongoMapperIdeable.m_id);

            var result = CollectionsManager.GetCollection<T>(Type.Name).DeleteOneAsync(query).GetAwaiter().GetResult();

            return result.DeletedCount > 0;

        }

        #endregion
    }
}