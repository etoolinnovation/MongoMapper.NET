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
        public void TestVersionInc()
        {
            Helper.DropAllCollections();

            List<Country> countries = new List<Country>();

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
    }
}
