using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void TestReadConfig()
        {
            ConfigManager.OverrideUrlString(string.Empty);

            MongoClientSettings conf = ConfigManager.GetClientSettings("TestConf1");

            Assert.AreEqual(3, conf.Servers.Count());
            Assert.AreEqual("host1:27017", conf.Servers.ToList()[0].ToString());
            Assert.AreEqual("host2:27017", conf.Servers.ToList()[1].ToString());
            Assert.AreEqual("host3:1234", conf.Servers.ToList()[2].ToString());

            Assert.AreEqual(ConfigManager.DataBaseName("TestConf1"), "Conf1");

            Assert.AreEqual(ReadPreference.PrimaryPreferred, conf.ReadPreference);
            Assert.AreEqual(conf.WriteConcern.W, WriteConcern.WValue.Parse("2"));
            //Assert.AreEqual(1, conf.MaxConnectionPoolSize);
            //Assert.AreEqual(TimeSpan.FromSeconds(2), conf.WaitQueueTimeout);
            //Assert.AreEqual(true, conf.WriteConcern.Journal);


            //Assert.AreEqual(
            //    "mongodb://127.0.0.1/TestDotNET?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;fireAndForget=true;journal=true",
            //    ConfigManager.GetConnectionString("Country"));

            //Assert.AreEqual(
            //    "mongodb://127.0.0.1/TestDotNETPerson?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;fireAndForget=false;journal=true",
            //    ConfigManager.GetConnectionString("Person"));

            //Assert.AreEqual(
            //    "mongodb://127.0.0.1/TestDotNET?w=1;maxpoolsize=50;waitQueueTimeout=1000ms;fireAndForget=true;journal=true",
            //    ConfigManager.GetConnectionString("XXX"));

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

        [Test]
        public void TestDatabasePrefix()
        {
            ConfigManager.DatabasePrefix = "Prefix";
            Assert.AreEqual(ConfigManager.DataBaseName("TestConf1"), "Prefix_Conf1");
        }
    }
}