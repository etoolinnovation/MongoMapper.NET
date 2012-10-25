namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::NUnit.Framework;

    [TestFixture]
    public class ConfigTest
    {
        #region Public Methods

        [Test]
        public void TestReadConfig()
        {
            ConfigManager.OverrideConnectionString(string.Empty);
            Assert.AreEqual(
                "mongodb://user:pass@host1,host2,host3:1234/Conf1?slaveOk=true;maxpoolsize=1;waitQueueTimeout=2000ms;safe=true;journal=true",
                ConfigManager.GetConnectionString("TestConf1"));

            Assert.AreEqual(
                "mongodb://127.0.0.1/TestDotNET?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;safe=false;journal=true",
                ConfigManager.GetConnectionString("Country"));

            Assert.AreEqual(
                "mongodb://127.0.0.1/TestDotNETPerson?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;journal=true",
                ConfigManager.GetConnectionString("Person"));

            Assert.AreEqual(
                "mongodb://127.0.0.1/TestDotNET?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;safe=false;journal=true",
                ConfigManager.GetConnectionString("XXX"));

            /*	
             *   Assert.AreEqual(ConfigManager.GetConnectionString("Country"),
                "mongodb://10.176.194.191,10.176.195.254,10.176.196.5/TestDotNET?replicaSet=dingusSet1;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;journal=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Person"),
                "mongodb://10.176.194.191,10.176.195.254,10.176.196.5/TestDotNETPerson?replicaSet=dingusSet1;w=2;slaveOk=true;maxpoolsize=50;waitQueueTimeout=2000ms;safe=true;journal=true");
            Assert.AreEqual(ConfigManager.GetConnectionString("Fake"), 
                "mongodb://user:pass@fake.com:27017/Test?maxpoolsize=100;waitQueueTimeout=2000ms;safe=true;journal=false");

             Assert.AreEqual(ConfigManager.GetConnectionString("XXX"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;journal=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Country"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNET?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=1000ms;safe=true;journal=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Person"),
            "mongodb://192.168.1.218:27017,192.168.1.216:27017,192.168.1.14:27017/TestDotNETPerson?replicaSet=devSet;w=1;slaveOk=true;maxpoolsize=50;waitQueueTimeout=2000ms;safe=true;journal=true");
        Assert.AreEqual(ConfigManager.GetConnectionString("Fake"), 
            "mongodb://user:pass@fake.com:27017/Test?maxpoolsize=100;waitQueueTimeout=2000ms;safe=true;journal=false");			
       */
        }

        #endregion
    }
}