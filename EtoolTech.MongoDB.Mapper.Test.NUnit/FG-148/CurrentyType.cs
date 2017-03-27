using System;
using System.Linq;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.IdGenerators;
using FindByKeyNotFoundException = EtoolTech.MongoDB.Mapper.Exceptions.FindByKeyNotFoundException;

namespace EtoolTech.Orca.BedBank.BlackBox.Core.Data.Mongo
{
    [Serializable]
    [MongoKey(KeyFields = "CurrencyCode")]
    [MongoMapperIdIncrementable(IncremenalId = false, ChildsIncremenalId = false)]
    public partial class CurrencyType : MongoMapper<CurrencyType>
    {

        private static readonly Dictionary<string, object> LocalCache = new Dictionary<string, object>();
        private object GetFromLocalCache<T>(string objectName, string fieldNames, params object[] values)
        {

            if (values.Any(value => value == null))
            {
                return null;
            }

            string valueKey = values.Aggregate(String.Empty, (current, value) => current + (value.ToString() + "|"));

            string key = (string.Format("{0}|{1}|{2}", objectName, fieldNames, valueKey));
            if (!LocalCache.ContainsKey(key))
            {
                var queryList = new List<FilterDefinition<T>>();
                int index = 0;
                var fields = fieldNames.Split(',');
                foreach (var value in values)
                {
                    FilterDefinition<T> query;
                    if (value is long)
                    {
                        query = MongoQuery<T>.Eq(objectName, fields[index], (long)value);
                    }
                    else if (value is int)
                    {
                        query = MongoQuery<T>.Eq(objectName, fields[index], (int)value);
                    }
                    else
                    {
                        query = MongoQuery<T>.Eq(objectName, fields[index], value.ToString());
                    }
                    queryList.Add(query);
                    index++;
                }

                var cursor = MongoMapperCollection<T>.Instance.Find(Builders<T>.Filter.And(queryList)).ToListAsync().Result;

                object data = null;
                if (cursor.Any()) data = cursor.First();

                if (!LocalCache.ContainsKey(key)) LocalCache.Add(key, data);
            }
            return LocalCache[key];
        }

        [BsonIgnore]
        public long _id { get { return base.m_id; } set { base.m_id = value; } }
        public String CurrencyCode { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Simbol { get; set; }
        public Decimal Value { get; set; }
        public String SermepaCode { get; set; }
        public Boolean IsLocalCurrency { get; set; }
        [BsonElement("lmu")]
        public string LastModificationUser { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("lmd")]
        public DateTime LastModificationDateTime { get; set; }


        public new void Save()
        {
            try
            {
                base.Save();
            }
            catch (EtoolTech.MongoDB.Mapper.Exceptions.DuplicateKeyException)
            {
                throw new DuplicateKeyException();
            }
        }

        public new void Delete()
        {
            base.Delete();
        }


    }

}
