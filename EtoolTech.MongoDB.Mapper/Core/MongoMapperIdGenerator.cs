namespace EtoolTech.MongoDB.Mapper
{
    using System;

    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public class MongoMapperIdGenerator : IIdGenerator
    {
        #region Constants and Fields

        private readonly Object _lockObject = new Object();

        #endregion

        #region Public Properties

        public static MongoMapperIdGenerator Instance
        {
            get
            {
                return new MongoMapperIdGenerator();
            }
        }

        #endregion

        #region Public Methods

        public object GenerateId(object container, object document)
        {
            string objName = document.GetType().Name;
            if (!ConfigManager.UseIncrementalId(objName))
            {
                return this.GenerateId();
            }

            return this.GenerateIncrementalId(objName);
        }

        public long GenerateId()
        {
            var baseDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime now = DateTime.UtcNow;
            var days = (ushort)(now - baseDate).TotalDays;
            var milliseconds = (int)now.TimeOfDay.TotalMilliseconds;
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

        public long GenerateIncrementalId(string objName)
        {
            lock (this._lockObject)
            {
                FindAndModifyResult result = this.FindAndModifyResult(objName);
                return result.ModifiedDocument["last"].AsInt64;
            }
        }

        public bool IsEmpty(object id)
        {
            return (long)id == default(long);
        }

        #endregion

        #region Methods

        private FindAndModifyResult FindAndModifyResult(string objName)
        {
            FindAndModifyResult result =
                Helper.Db("Counters", true).GetCollection("Counters").FindAndModify(
                    Query.EQ("document", objName), null, Update.Inc("last", (long)1), true, true);
            return result;
        }

        #endregion
    }
}