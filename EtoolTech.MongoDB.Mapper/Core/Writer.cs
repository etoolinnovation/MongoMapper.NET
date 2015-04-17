using System;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Writer : IWriter
    {
        internal static IWriter Instance
        {
            get { return new Writer(); }
        }

        #region IWriter Members

        public WriteConcernResult Insert(string Name, Type Type, object Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Insert, Type, Document);
                return new WriteConcernResult(new BsonDocument());
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            WriteConcernResult result = CollectionsManager.GetCollection(Name).Insert(Type, Document);


            if (result != null && result.HasLastErrorMessage)
            {
                throw new Exception(result.LastErrorMessage);
            }

            return result;
        }

        public WriteConcernResult Update(string Name, Type Type, object Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Update, Type, Document);
                return new WriteConcernResult(new BsonDocument());
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.m_dv++;
            }

            WriteConcernResult result = CollectionsManager.GetCollection(Name).Save(Type, Document);

            if (result != null && result.HasLastErrorMessage)
            {
                throw new Exception(result.LastErrorMessage);
            }

            return result;
        }

        public WriteConcernResult Delete(string Name, Type Type, object Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Delete, Type, Document);
                return new WriteConcernResult(new BsonDocument());
            }


            if (((MongoMapper) Document).m_id == default(long))
            {
                ((MongoMapper) Document).m_id = Finder.Instance.FindIdByKey(Type,
                                                                            MongoMapperHelper.GetPrimaryKey(Type).
                                                                                ToDictionary(
                                                                                    KeyField => KeyField,
                                                                                    KeyField =>
                                                                                    ReflectionUtility.
                                                                                        GetPropertyValue(
                                                                                            this, KeyField))
                    );
            }

            IMongoQuery query = Query.EQ("_id", ((MongoMapper) Document).m_id);

            WriteConcernResult result =
                CollectionsManager.GetCollection(Type.Name).Remove(
                    query);


            if (result != null && result.HasLastErrorMessage)
            {
                throw new Exception(result.LastErrorMessage);
            }


            return result;
        }

        #endregion
    }
}