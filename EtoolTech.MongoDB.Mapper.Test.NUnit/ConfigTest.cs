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
                "mongodb://10.176.194.191,10.176.195.254,10.176.196.5/TestDotNET?replicaSet=dingusSet1;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Country"),
                "mongodb://10.176.194.191,10.176.195.254,10.176.196.5/TestDotNET?replicaSet=dingusSet1;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Person"),
                "mongodb://10.176.194.191,10.176.195.254,10.176.196.5/TestDotNETPerson?replicaSet=dingusSet1;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=2000ms;safe=true;fsync=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Fake"), 
                "mongodb://user:pass@fake.com:27017/Test?maxpoolsize=100;waitQueueTimeout=2000ms;safe=true;fsync=false");


            /*	
             Assert.AreEqual(ConfigManager.GetConnectionString("XXX"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Country"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;fsync=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Person"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNETPerson?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=2000ms;safe=true;fsync=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Fake"), 
            "mongodb://user:pass@fake.com:27017/Test?maxpoolsize=100;waitQueueTimeout=2000ms;safe=true;fsync=false");			
       */

        }
    }
}
