using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class EventsTest
    {
        //private MongoTestServer _mongoTestServer;

        //[TestFixtureSetUp]
        //public void Init()
        //{
        //    MongoTestServer.SetMongodPtah(@"mongod\");
        //    this._mongoTestServer = MongoTestServer.Start(27017);
        //    ConfigManager.OverrideUrlString(this._mongoTestServer.ConnectionString);
        //}

        //[TestFixtureTearDown]
        //public void Dispose()
        //{
        //    this._mongoTestServer.Dispose();
        //}

        [Test]
        public void TestEvents()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "FR", Name = "España"};
            c.OnBeforeInsert += (s, e) => { ((Country) s).Name = "Francia"; };
            c.Save();

            var c3 = MongoMapper.FindByKey<Country>("FR");

            Assert.AreEqual(c3.Name, "Francia");
        }
    }
}