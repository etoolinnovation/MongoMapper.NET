namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using global::NUnit.Framework;

    [TestFixture]
    public class EventsTest
    {
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