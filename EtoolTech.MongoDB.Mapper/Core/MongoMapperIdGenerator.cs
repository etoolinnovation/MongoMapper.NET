using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class MongoMapperIdGenerator : IIdGenerator 
    {
        public static MongoMapperIdGenerator Instance {get{return new MongoMapperIdGenerator();}}
        
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
            var result = FindAndModifyResult(objName);

            if (result.ModifiedDocument == null)
            {
                lock (this)
                {
                    result = FindAndModifyResult(objName);

                    if (result.ModifiedDocument == null)
                    {
                        BsonDocument counter = new BsonDocument
                                                   {
                                                       {"document", objName},
                                                       {"last", (long) 1}
                                                   };
                        Helper.Db.GetCollection("Counters").Insert(counter);
                        return (long) 1;
                    }
                }
            }

            return (long) result.ModifiedDocument["last"];
        }

        private static FindAndModifyResult FindAndModifyResult(string objName)
        {
            var result =
                Helper.Db.GetCollection("Counters").FindAndModify(Query.EQ("document", objName),
                                                                  null,
                                                                  Update.Inc("last", 1), true);
            return result;
        }
      

        public bool IsEmpty(object id)
        {
            return (long) id == default(long);
        }
    }
}
