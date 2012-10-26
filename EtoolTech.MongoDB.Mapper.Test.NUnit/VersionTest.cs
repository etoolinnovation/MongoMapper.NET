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

        public void TestIsLastVersion()
        {
            Helper.DropAllCollections();
            

            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            Assert.AreEqual(true, c.IsLastVersion());

            //Esto solo funciona en un replica set, lo dejo comentado
            //c.MongoMapperDocumentVersion = 99;
            //Assert.AreEqual(false, c.IsLastVersion());

        }

        public void TestFillFromLastVersion()
        {
            Helper.DropAllCollections();


            var c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            Assert.AreEqual(true, c.IsLastVersion());
            
            c.MongoMapperDocumentVersion = 99;
            c.FillFromLastVersion();
            Assert.AreEqual(1, c.MongoMapperDocumentVersion);

        }
    }
}
