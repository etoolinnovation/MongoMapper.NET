namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::NUnit.Framework;

    [TestFixture]
    public class EventsTest
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
        
        #region Public Methods

        [Test]
        public void TestEvents()
        {
            Helper.DropAllCollections();

            var c = new Country { Code = "FR", Name = "España" };
            c.OnBeforeInsert += (s, e) => { ((Country)s).Name = "Francia"; };
            c.Save<Country>();

            var c3 = MongoMapper.FindByKey<Country>("FR");

            Assert.AreEqual(c3.Name, "Francia");
        }

        #endregion
    }
}