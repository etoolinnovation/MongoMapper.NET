using System;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperIdGenerator : IIdGenerator
    {
        private readonly Object lockObject = new Object();

        public static MongoMapperIdGenerator Instance
        {
            get { return new MongoMapperIdGenerator(); }
        }

        #region IIdGenerator Members

        public object GenerateId(object container, object document)
        {
            string objName = document.GetType().Name;
            if (!ConfigManager.UserIncrementalId(objName))
            {
                var id = (ObjectId) ObjectIdGenerator.Instance.GenerateId(container, document);
                return (long) id.GetHashCode();
            }

            return GenerateIncrementalId(objName);
        }

        public bool IsEmpty(object id)
        {
            return (long) id == default(long);
        }

        #endregion

        public long GenerateIncrementalId(string objName)
        {
            lock (lockObject)
            {
                FindAndModifyResult result = FindAndModifyResult(objName);
                return result.ModifiedDocument["last"].AsInt64;
            }
        }

        private FindAndModifyResult FindAndModifyResult(string objName)
        {
            FindAndModifyResult result = Helper.Db("Counters").GetCollection("Counters").FindAndModify(
                Query.EQ("document", objName), null, Update.Inc("last", (long) 1), true, true);
            return result;
        }
    }
}