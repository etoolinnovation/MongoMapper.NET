namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NUnit.Framework;

    [TestFixture]
    public class MongoTestServerTest
    {
        #region Public Methods

        [Test]
        public void TestMoreThanDifSamePort()
        {
            MongoTestServer.SetMongodPtah(@"mongod\");
            using (MongoTestServer mongoTestServer = MongoTestServer.Start(27017))
            {
                using (MongoTestServer mongoTestServer2 = MongoTestServer.Start(27018))
                {
                    Assert.AreEqual(2, MongoTestServer.InstancesByPort.Count);
                }
                Assert.AreEqual(1, MongoTestServer.InstancesByPort.Count);
            }
            Assert.AreEqual(0, MongoTestServer.InstancesByPort.Count);
        }

        [Test]
        public void TestMoreThanOneSamePort()
        {
            MongoTestServer.SetMongodPtah(@"mongod\");
            using (MongoTestServer mongoTestServer = MongoTestServer.Start(27017))
            {
                using (MongoTestServer mongoTestServer2 = MongoTestServer.Start(27017))
                {
                    List<KeyValuePair<int, MongoTestInstance>> instance =
                        (from i in MongoTestServer.InstancesByPort where i.Key == 27017 select i).ToList();
                    Assert.AreEqual(1, instance.Count);
                    using (MongoTestServer mongoTestServer3 = MongoTestServer.Start(27017))
                    {
                        List<KeyValuePair<int, MongoTestInstance>> instance2 =
                            (from i in MongoTestServer.InstancesByPort where i.Key == 27017 select i).ToList();
                        Assert.AreEqual(1, instance.Count);
                        using (MongoTestServer mongoTestServer4 = MongoTestServer.Start(27017))
                        {
                            List<KeyValuePair<int, MongoTestInstance>> instance3 =
                                (from i in MongoTestServer.InstancesByPort where i.Key == 27017 select i).ToList();
                            Assert.AreEqual(1, instance.Count);
                            using (MongoTestServer mongoTestServer5 = MongoTestServer.Start(27017))
                            {
                                List<KeyValuePair<int, MongoTestInstance>> instance4 =
                                    (from i in MongoTestServer.InstancesByPort where i.Key == 27017 select i).ToList();
                                Assert.AreEqual(1, instance.Count);
                                using (MongoTestServer mongoTestServer6 = MongoTestServer.Start(27017))
                                {
                                    List<KeyValuePair<int, MongoTestInstance>> instance5 =
                                        (from i in MongoTestServer.InstancesByPort where i.Key == 27017 select i).ToList
                                            ();
                                    Assert.AreEqual(1, instance.Count);
                                }
                            }
                        }
                    }
                }
                Assert.AreEqual(1, MongoTestServer.InstancesByPort.Count);
            }
            Assert.AreEqual(0, MongoTestServer.InstancesByPort.Count);
        }

        #endregion
    }
}