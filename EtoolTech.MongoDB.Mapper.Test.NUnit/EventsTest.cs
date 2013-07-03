using System.Text;
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


        public void TextTest()
        {
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes("1Ñ");
            string s = System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}