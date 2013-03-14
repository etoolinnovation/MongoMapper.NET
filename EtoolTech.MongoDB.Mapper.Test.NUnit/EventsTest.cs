using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class EventsTest
    {
      
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