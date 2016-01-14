using System;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperIdGenerator : IIdGenerator
    {
        #region Constants and Fields

        private readonly Object _lockObject = new Object();

        #endregion

        #region Public Properties

        public static MongoMapperIdGenerator Instance
        {
            get { return new MongoMapperIdGenerator(); }
        }

        #endregion

        #region Public Methods

        public object GenerateId(object Container, object Document)
        {
            string objName = Document.GetType().Name;
            if (!ConfigManager.UseIncrementalId(objName))
            {
                return GenerateId();
            }

            return GenerateIncrementalId(objName);
        }

        public bool IsEmpty(object Id)
        {
            return (long) Id == default(long);
        }

        public long GenerateId()
        {
            var baseDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime now = DateTime.UtcNow;
            var days = (ushort) (now - baseDate).TotalDays;
            var milliseconds = (int) now.TimeOfDay.TotalMilliseconds;
            byte[] bytes = Guid.NewGuid().ToByteArray();

            Array.Copy(BitConverter.GetBytes(days), 0, bytes, 10, 2);
            Array.Copy(BitConverter.GetBytes(milliseconds), 0, bytes, 12, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes, 10, 2);
                Array.Reverse(bytes, 12, 4);
            }

            return BitConverter.ToInt64(bytes, 0);
        }

        public long GenerateIncrementalId(string ObjName)
        {
            lock (_lockObject)
            {
                return FindAndModifyResult(ObjName);                
            }
        }

        #endregion

        #region Methods

        private long FindAndModifyResult(string ObjName)
        {      
            var result = MongoMapperHelper.Db("Counters", true)
                .GetCollection<BsonDocument>("Counters")
                .FindOneAndUpdateAsync(
                    Builders<BsonDocument>.Filter.Eq("document", ObjName),
                    Builders<BsonDocument>.Update.Inc("last", (long) 1), 
                    new FindOneAndUpdateOptions<BsonDocument>() { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                ).Result;

            return result["last"].AsInt64;
        }

        #endregion
    }
}