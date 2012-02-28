using NUnit.Framework;
using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void TestReadConfig()
        {
            Assert.AreEqual(ConfigManager.GetConnectionString("XXX"),
                "mongodb://192.168.1.214:27017,192.168.1.216:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Country"),
                "mongodb://192.168.1.214:27017,192.168.1.216:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Person"),
                "mongodb://192.168.1.214:27017,192.168.1.216:27017/TestDotNETPerson?replicaSet=devSet;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=2000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Fake"), 
                "mongodb://user:pass@fake.com:27017/Test?maxpoolsize=100;waitQueueTimeout=2000ms;safe=true;fsync=false");			
        }
    }
}
