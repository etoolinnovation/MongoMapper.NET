namespace EtoolTech.MongoDB.Mapper
{
    using System;

    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Bson.Serialization.IdGenerators;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public class MongoMapperIdGenerator : IIdGenerator
    {
        private Object lockObject = new Object();

        public static MongoMapperIdGenerator Instance
        {
            get
            {
                return new MongoMapperIdGenerator();
            }
        }

        public object GenerateId(object container, object document)
        {
            if (!ConfigManager.UserIncrementalId)
            {
                ObjectId id = (ObjectId)ObjectIdGenerator.Instance.GenerateId(container, document);
                return id.GetHashCode();
            }
            else
            {
                return this.GenerateIncrementalId(document.GetType().Name);
            }
        }

        public long GenerateIncrementalId(string objName)
        {
            lock (this.lockObject)
            {
                var result = this.FindAndModifyResult(objName);
                return result.ModifiedDocument["last"].AsInt64;
            }
        }

        private FindAndModifyResult FindAndModifyResult(string objName)
        {
            var result = Helper.Db.GetCollection("Counters").FindAndModify(
                Query.EQ("document", objName), null, Update.Inc("last", (long)1), true, true);
            return result;
        }

        public bool IsEmpty(object id)
        {
            return (long)id == default(long);
        }
    }
}