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
            c.Save<Country>();
            Assert.AreEqual(1,c.MongoMapperDocumentVersion);
            countries.MongoFind();
            Assert.AreEqual(1,countries.Count);

            c.Save<Country>();
            Assert.AreEqual(2, c.MongoMapperDocumentVersion);
            countries.MongoFind();
            Assert.AreEqual(1, countries.Count);

            c.Save<Country>();
            Assert.AreEqual(3, c.MongoMapperDocumentVersion);
            countries.MongoFind();
            Assert.AreEqual(1, countries.Count);


        }

        [Test]
        public void TestIsLastVersion()
        {
            Helper.DropAllCollections();
            

            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            Assert.AreEqual(true, c.IsLastVersion());
            
            c.MongoMapperDocumentVersion = 99;
            Assert.AreEqual(false, c.IsLastVersion(true));

        }

        [Test]
        public void TestFillFromLastVersion()
        {
            Helper.DropAllCollections();


            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            Assert.AreEqual(true, c.IsLastVersion());
            
            c.MongoMapperDocumentVersion = 99;
            c.FillFromLastVersion(true);
            Assert.AreEqual(1, c.MongoMapperDocumentVersion);

        }
    }
}
