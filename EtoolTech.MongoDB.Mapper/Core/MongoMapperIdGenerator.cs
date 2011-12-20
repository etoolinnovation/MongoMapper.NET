using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class MongoMapperIdGenerator : IIdGenerator
    {
        private Object lockObject = new Object();

        public static MongoMapperIdGenerator Instance
        {
            get { return new MongoMapperIdGenerator(); }
        }

        public object GenerateId(object container, object document)
        {
            if (!Helper.UserIncrementalId)
            {
                ObjectId id = (ObjectId)ObjectIdGenerator.Instance.GenerateId(container, document);
                return id.GetHashCode();
            }
            else
            {
                return GenerateIncrementalId(document.GetType().Name);
            }
        }

        public long GenerateIncrementalId(string objName)
        {
            lock (lockObject)
            {
                var result = FindAndModifyResult(objName);           
                return result.ModifiedDocument["last"].AsInt64;
            }
        }

        private FindAndModifyResult FindAndModifyResult(string objName)
        {
            var result =
                Helper.Db.GetCollection("Counters").FindAndModify(Query.EQ("document", objName),
                                                                  null,
                                                                  Update.Inc("last", (long) 1), true, true);
            return result;
        }


        public bool IsEmpty(object id)
        {
            return (long)id == default(long);
        }

   
    }
}