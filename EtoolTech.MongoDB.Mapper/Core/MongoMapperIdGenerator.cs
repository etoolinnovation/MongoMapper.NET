using System;
using System.Collections.Generic;
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
        private readonly Object _lockObject = new Object();
      
        public static MongoMapperIdGenerator Instance
        {
            get { return new MongoMapperIdGenerator(); }
        }

        #region IIdGenerator Members

        public object GenerateId(object container, object document)
        {
            string objName = document.GetType().Name;
            if (!ConfigManager.UseIncrementalId(objName))
            {                
                return GenerateId();
            }

            return GenerateIncrementalId(objName);
        }

        public bool IsEmpty(object id)
        {
            return (long) id == default(long);
        }

        #endregion
		
		public long GenerateId()
		{
			var baseDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = DateTime.UtcNow;
            var days = (ushort)(now - baseDate).TotalDays;
            var milliseconds = (int)now.TimeOfDay.TotalMilliseconds;  
            var bytes = Guid.NewGuid().ToByteArray();
			
            Array.Copy(BitConverter.GetBytes(days), 0, bytes, 10, 2);
            Array.Copy(BitConverter.GetBytes(milliseconds), 0, bytes, 12, 4);
			
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes, 10, 2);
                Array.Reverse(bytes, 12, 4);
            }
			
			return BitConverter.ToInt64(bytes,0);
		}

        public long GenerateIncrementalId(string objName)
        {            
            lock(_lockObject)
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