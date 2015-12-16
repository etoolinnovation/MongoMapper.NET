using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [Serializable]
    [MongoKey(KeyFields = "Code,Data")]
    [MongoMapperIdIncrementable(IncremenalId = true, ChildsIncremenalId = false)]
    [MongoTTLIndex(IndexField = "Date", Seconds = 600)]
    public class Log: MongoMapper<Log>
    {
        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public string Data { get; set; }
    }
}
