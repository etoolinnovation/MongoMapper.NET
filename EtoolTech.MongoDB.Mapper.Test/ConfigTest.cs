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

            Assert.AreEqual(config.Server.Host,"192.168.1.214");
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

        }
    }
}
