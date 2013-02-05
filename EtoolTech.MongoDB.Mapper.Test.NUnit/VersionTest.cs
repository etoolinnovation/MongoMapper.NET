using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class VersionTest
    {
        [Test]
        public void TestVersionInc()
        {
            Helper.DropAllCollections();

            var countries = new List<Country>();

            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save();
            Assert.AreEqual(1,c.m_dv);
            countries.MongoFind();
            Assert.AreEqual(1,countries.Count);

            c.Save();
            Assert.AreEqual(2, c.m_dv);
            countries.MongoFind();
            Assert.AreEqual(1, countries.Count);

            c.Save();
            Assert.AreEqual(3, c.m_dv);
            countries.MongoFind();
            Assert.AreEqual(1, countries.Count);


        }

        [Test]
        public void TestIsLastVersion()
        {
            Helper.DropAllCollections();
            

            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save();

            Assert.AreEqual(true, c.IsLastVersion());
            
            c.m_dv = 99;
            Assert.AreEqual(false, c.IsLastVersion(true));

        }

        [Test]
        public void TestFillFromLastVersion()
        {
            Helper.DropAllCollections();


            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save();

            Assert.AreEqual(true, c.IsLastVersion());
            
            c.m_dv = 99;
            c.FillFromLastVersion(true);
            Assert.AreEqual(1, c.m_dv);

        }
    }
}
