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
                string documentName = document.GetType().Name;
                MongoCollection colection = Helper.Db.GetCollection("Counters");

                var result = FindAndModifyResult(colection, documentName);

                if (result.ModifiedDocument == null)
                {
                    lock (this)
                    {
                        result = FindAndModifyResult(colection, documentName);

                        if (result.ModifiedDocument == null)
                        {
                            InsertCounter(colection, documentName);
                            return (long) 1;
                        }
                    }
                }

                return (long) result.ModifiedDocument["last"];
            }
        }

        private static void InsertCounter(MongoCollection colection, string documentName)
        {
            BsonDocument counter = new BsonDocument { { "document", documentName }, { "last", (long) 1 } };
            colection.Insert(counter);
        }

        private static FindAndModifyResult FindAndModifyResult(MongoCollection colection, string documentName)
        {
            var result = colection.FindAndModify(Query.EQ("document", documentName), null, Update.Inc("last", 1), true);
            return result;
        }
      

        public bool IsEmpty(object id)
        {
            return (long) id == default(long);
        }
    }
}
