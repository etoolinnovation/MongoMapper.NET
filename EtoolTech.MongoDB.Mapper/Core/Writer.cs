using System;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Writer : IWriter
    {
        internal static IWriter Instance
        {
            get
            {
                return new Writer();
            }
        }

        public SafeModeResult Insert(string name, Type type, object document)
        {

            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Insert, type, document);
                return new SafeModeResult();
            }

            SafeModeResult result = CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(name)).Insert(type, document);

            if (ConfigManager.SafeMode(type.Name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            return result;
        }

        public SafeModeResult Update(string name, Type type, object document)
        {
            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Update, type, document);
                return new SafeModeResult();
            }
            
            SafeModeResult result = CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(name)).Save(type, document);

            if (ConfigManager.SafeMode(name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            return result;
        }

        public SafeModeResult Delete(string name, Type type, object document)
        {
            throw new NotImplementedException();

            if (MongoMapperTransaction.InTransaction && !MongoMapperTransaction.Commiting)
            {
                MongoMapperTransaction.AddToQueue(OperationType.Delete, type, document);
                return new SafeModeResult();
            }


            //TODO: Pendiente ver como solucionar el tema el T
            //if ( ((MongoMapper)queue.Document).MongoMapper_Id == default(long))
            //{
            //    ((MongoMapper)queue.Document).MongoMapper_Id = Finder.Instance.FindIdByKey<T>(
            //        Helper.GetPrimaryKey(queue.Type).ToDictionary(keyField => keyField, keyField => ReflectionUtility.GetPropertyValue(this, keyField))
            //        );
            //}

            //IMongoQuery query = Query.EQ("_id", ((MongoMapper)queue.Document).MongoMapper_Id);

            //SafeModeResult result =
            //    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(queue.Type.Name)).Remove(
            //        query);

            //if (ConfigManager.SafeMode(queue.Type.Name))
            //{
            //    if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
            //    {
            //        throw new DeleteDocumentException(result.ErrorMessage);
            //    }
            //}
        }

    
    }
}
