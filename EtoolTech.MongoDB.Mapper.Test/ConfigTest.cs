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

            Assert.AreEqual(ConfigManager.GetConnectionString("XXX"), "mongodb://192.168.1.214:27017/TestDotNET?connect=direct;maxpoolsize=5;waitQueueTimeout=1000ms;safe=true;fsync=false");
            Assert.AreEqual(ConfigManager.GetConnectionString("Country"), "mongodb://192.168.1.214:27017/TestDotNET?connect=direct;maxpoolsize=5;waitQueueTimeout=1000ms;safe=true;fsync=false");
            Assert.AreEqual(ConfigManager.GetConnectionString("Person"), "mongodb://192.168.1.214:27017/TestDotNETPerson?connect=direct;maxpoolsize=10;waitQueueTimeout=2000ms;safe=true;fsync=true");

        }
    }
}
