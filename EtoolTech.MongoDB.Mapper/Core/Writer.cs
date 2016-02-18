using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public Task InsertAsync<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Insert, Type, Document);
                Task.FromResult(true);
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            return CollectionsManager.GetCollection<T>(Name).InsertOneAsync(Document);
        }

        public bool Insert<T>(string Name, Type Type, T Document)
        {
            InsertAsync(Name, Type, Document).GetAwaiter().GetResult();
            return true;

        }

        public Task<ReplaceOneResult> UpdateAsync<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Update, Type, Document);
                Task.FromResult(true);
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            var mongoMapperIdeable = Document as IMongoMapperIdeable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            Debug.Assert(mongoMapperIdeable != null, "mongoMapperIdeable != null");

            return CollectionsManager.GetCollection<T>(Name).ReplaceOneAsync(
                Builders<T>.Filter.Eq("_id", mongoMapperIdeable.m_id),
                Document,
                new UpdateOptions {IsUpsert = true}
                );
        }

        public bool Update<T>(string Name, Type Type, T Document)
        {

            var result = UpdateAsync(Name, Type, Document).GetAwaiter().GetResult();            
            return result.ModifiedCount > 0;
        }

        public Task<DeleteResult> DeleteAsync<T>(string Name, Type Type, T Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Delete, Type, Document);
                Task.FromResult(true);
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

            return CollectionsManager.GetCollection<T>(Type.Name).DeleteOneAsync(query);
        }

        public bool Delete<T>(string Name, Type Type, T Document)
        {

            var result = DeleteAsync(Name, Type, Document).GetAwaiter().GetResult();

            return result.DeletedCount > 0;

        }

        #endregion
    }
}