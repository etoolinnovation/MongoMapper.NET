using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class MongoMapperIdGenerator : IIdGenerator 
    {
        public object GenerateId(object container, object document)
        {
            if (!Helper.UserIncrementalId)
            {
                ObjectId id = (ObjectId)ObjectIdGenerator.Instance.GenerateId(container, document);
                return id.GetHashCode();
            }
            else
            {
                var result = FindAndModifyResult(document);

                if (result.ModifiedDocument == null)
                {
                    lock (this)
                    {
                        result = FindAndModifyResult(document);

                        if (result.ModifiedDocument == null)
                        {
                            BsonDocument counter = new BsonDocument
                                                       {
                                                           {"document", document.GetType().Name},
                                                           {"last", (long) 1}
                                                       };
                            Helper.Db.GetCollection("Counters").Insert(counter);
                            return (long) 1;
                        }
                    }
                }

                return (long) result.ModifiedDocument["last"];
            }
        }

        private static FindAndModifyResult FindAndModifyResult(object document)
        {
            var result =
                Helper.Db.GetCollection("Counters").FindAndModify(Query.EQ("document", document.GetType().Name),
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
