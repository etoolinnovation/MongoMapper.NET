using System;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
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
                return new WriteConcernResult();
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.MongoMapperDocumentVersion++;
            }

            WriteConcernResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(Name)).Insert(Type, Document);

            if (ConfigManager.SafeMode(Type.Name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            return result;
        }

        public WriteConcernResult Update(string Name, Type Type, object Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Update, Type, Document);
                return new WriteConcernResult();
            }

            var mongoMapperVersionable = Document as IMongoMapperVersionable;
            if (mongoMapperVersionable != null)
            {
                mongoMapperVersionable.MongoMapperDocumentVersion++;
            }

            WriteConcernResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(Name)).Save(Type, Document);

            if (ConfigManager.SafeMode(Name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            return result;
        }

        public WriteConcernResult Delete(string Name, Type Type, object Document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Delete, Type, Document);
                return new WriteConcernResult();
            }


            if (((MongoMapper) Document).MongoMapper_Id == default(long))
            {
                ((MongoMapper) Document).MongoMapper_Id = Finder.Instance.FindIdByKey(Type,
                                                                                      Helper.GetPrimaryKey(Type).
                                                                                          ToDictionary(
                                                                                              KeyField => KeyField,
                                                                                              KeyField =>
                                                                                              ReflectionUtility.
                                                                                                  GetPropertyValue(
                                                                                                      this, KeyField))
                    );
            }

            IMongoQuery query = Query.EQ("_id", ((MongoMapper) Document).MongoMapper_Id);

            WriteConcernResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(Type.Name)).Remove(
                    query);

            if (ConfigManager.SafeMode(Type.Name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new DeleteDocumentException(result.ErrorMessage);
                }
            }

            return result;
        }

        #endregion
    }
}