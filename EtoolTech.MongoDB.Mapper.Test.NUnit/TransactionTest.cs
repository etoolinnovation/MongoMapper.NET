using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Exceptions;

    using global::NUnit.Framework;


    [TestFixture]
    public class TransactionTest
    {

        private MongoTestServer _mongoTestServer;

        [TestFixtureSetUp]
        public void Init()
        {
            MongoTestServer.SetMongodPtah(@"mongod\");
            this._mongoTestServer = MongoTestServer.Start(27017);
            ConfigManager.OverrideConnectionString(this._mongoTestServer.ConnectionString);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            this._mongoTestServer.Dispose();
        }
        
        [Test]
        public void TestAddingSaveToQueue()
        {
            Helper.DropAllCollections();

            using (var t = new MongoMapperTransaction())
            {
                var c = new Country { Code = "NL", Name = "Holanda" };
                c.Save<Country>();
                var c2 = new Country { Code = "ES", Name = "España" };
                c2.Save<Country>();
                var c3 = new Country { Code = "US", Name = "USA" };
                c3.Save<Country>();

                Assert.AreEqual(3, t.QueueLenght);

                t.RollBack();

                Assert.AreEqual(0, t.QueueLenght);
            }

            var countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(0,countries.Count);

          
        }

        [Test]
        public void TestCommitingQueue()
        {
            Helper.DropAllCollections();

            using (var t = new MongoMapperTransaction())
            {
                var c = new Country { Code = "NL", Name = "Holanda" };
                c.Save<Country>();
                var c2 = new Country { Code = "ES", Name = "España" };
                c2.Save<Country>();
                var c3 = new Country { Code = "US", Name = "USA" };
                c3.Save<Country>();

                Assert.AreEqual(3, t.QueueLenght);

                t.Commit();

                Assert.AreEqual(0, t.QueueLenght);
            }

            var countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(3, countries.Count);

        }

        [Test]
        public void DuplicateTransaction()
        {
            using(var t = new MongoMapperTransaction())
            {
                try
                {
                    using (var t2 = new MongoMapperTransaction())
                    {

                    }
                }
                catch (Exception e)
                {
                    
                    Assert.AreEqual(typeof(DuplicateTransaction), e.GetType());
                }
            }
        }
    }
}
