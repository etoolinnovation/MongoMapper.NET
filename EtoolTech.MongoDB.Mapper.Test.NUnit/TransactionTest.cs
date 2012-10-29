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

        //private MongoTestServer _mongoTestServer;

        //[TestFixtureSetUp]
        //public void Init()
        //{
        //    MongoTestServer.SetMongodPtah(@"mongod\");
        //    this._mongoTestServer = MongoTestServer.Start(27017);
        //    ConfigManager.OverrideConnectionString(this._mongoTestServer.ConnectionString);
        //}

        //[TestFixtureTearDown]
        //public void Dispose()
        //{
        //    this._mongoTestServer.Dispose();
        //}
        
        [Test]
        public void TestAddingSaveToQueue()
        {
            Helper.DropAllCollections();

            using (var t = new MongoMapperTransaction())
            {
                var c = new Country { Code = "NL", Name = "Holanda" };
                c.Save<Country>();
                var countries1 = new List<Country>();
                countries1.MongoFind();
                Assert.AreEqual(0, countries1.Count);

                var c2 = new Country { Code = "ES", Name = "España" };
                c2.Save<Country>();
                var countries2 = new List<Country>();
                countries2.MongoFind();
                Assert.AreEqual(0, countries2.Count);

                var c3 = new Country { Code = "US", Name = "USA" };
                c3.Save<Country>();
                var countries3 = new List<Country>();
                countries3.MongoFind();
                Assert.AreEqual(0, countries3.Count);

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
                var countries1 = new List<Country>();
                countries1.MongoFind();
                Assert.AreEqual(0, countries1.Count);

                var c2 = new Country { Code = "ES", Name = "España" };
                c2.Save<Country>();
                var countries2 = new List<Country>();
                countries2.MongoFind();
                Assert.AreEqual(0, countries2.Count);

                var c3 = new Country { Code = "US", Name = "USA" };
                c3.Save<Country>();
                var countries3 = new List<Country>();
                countries3.MongoFind();
                Assert.AreEqual(0, countries3.Count);
                Assert.AreEqual(3, t.QueueLenght);                

                var countries4 = new List<Country>();
                countries4.MongoFind();
                Assert.AreEqual(0, countries4.Count);

                t.Commit();

                Assert.AreEqual(0, t.QueueLenght);
            }

            var countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(3, countries.Count);

        }

        [Test]
        public void TestErrorInCommitingQueue()
        {
            Helper.DropAllCollections();

            using (var t = new MongoMapperTransaction())
            {
                try
                {
                    var c = new Country { Code = "NL", Name = "Holanda" };
                    c.Save<Country>();
                    var countries1 = new List<Country>();
                    
                    countries1.MongoFind();
                    Assert.AreEqual(0, countries1.Count);
            
                    //Lanzara excepcion porque us esta en minusculas
                    var c3 = new Country { Code = "us", Name = "USA" };
                    c3.Save<Country>();

                    var countries3 = new List<Country>();
                    countries3.MongoFind();
                    Assert.AreEqual(0, countries3.Count);
                    Assert.AreEqual(2, t.QueueLenght);

                    t.Commit();
                }
                catch
                {                                     
                }


            }

            //No deberia haber guardado nada
            var countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(0, countries.Count);

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
