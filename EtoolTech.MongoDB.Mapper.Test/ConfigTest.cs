using EtoolTech.MongoDB.Mapper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestReadConfig()
        {
            MongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();

            Assert.AreEqual(config.Server.Host,"127.0.0.1");
            Assert.AreEqual(config.Server.Port, 27017);
            Assert.AreEqual(config.Database.User, "");
            Assert.AreEqual(config.Database.Password, "");
            Assert.AreEqual(config.Database.Name, "TestDotNET");
            Assert.AreEqual(config.Server.PoolSize, 5);
            Assert.AreEqual(config.Context.Generated,true);
            Assert.AreEqual(config.Server.WaitQueueTimeout, 1);
            Assert.AreEqual(config.Context.ExceptionOnDuplicateKey,true);
            Assert.AreEqual(config.Context.SafeMode, true);
            Assert.AreEqual(config.Context.EnableOriginalObject, true);
            Assert.AreEqual(config.Context.UserIncrementalId, true);
            Assert.AreEqual(config.Context.FSync, false);
            Assert.AreEqual(config.Context.MaxDocumentSize,8);
            

            int index = 1;
            foreach (CollectionElement collection in config.CollectionConfig)
            {
                Assert.AreEqual(index.ToString(),collection.Name);
                index++;

                Assert.AreEqual(collection.Server.Host, "127.0.0.1");
                Assert.AreEqual(collection.Server.Port, 27017);
                Assert.AreEqual(collection.Database.User, "");
                Assert.AreEqual(collection.Database.Password, "");
                Assert.AreEqual(collection.Database.Name, "TestDotNET");
                Assert.AreEqual(collection.Server.PoolSize, 5);
                Assert.AreEqual(collection.Context.Generated, true);
                Assert.AreEqual(collection.Server.WaitQueueTimeout, 1);
                Assert.AreEqual(collection.Context.ExceptionOnDuplicateKey, true);
                Assert.AreEqual(collection.Context.SafeMode, true);
                Assert.AreEqual(collection.Context.EnableOriginalObject, true);
                Assert.AreEqual(collection.Context.UserIncrementalId, true);
                Assert.AreEqual(collection.Context.FSync, false);
                Assert.AreEqual(collection.Context.MaxDocumentSize, 8);
                
            }

        }
    }
}
