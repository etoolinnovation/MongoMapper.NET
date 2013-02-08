using System;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class MongoCollectionTest
    {
        [Test]
        public void Tester()
        {
            Helper.DropAllCollections();

            
            var country = new Country { Code = "NL", Name = "Holanda" };
            country.Save();
            country = new Country { Code = "UK", Name = "Reino Unido" };
            country.Save();
            country = new Country { Code = "ES", Name = "España" };
            country.Save();
            
            var col = new MongoMapperCollection<Country>();
            col.Find().SetLimit(1);

            Console.WriteLine(col.Cursor.Explain().ToJson());

            Assert.AreEqual(1,col.Count);

            col = new MongoMapperCollection<Country>();
            col.Find().SetLimit(3);

            Assert.AreEqual(3, col.Count);

            Assert.AreEqual("NL",col.First().Code);

            foreach (Country c in col)
            {
                Console.Write(c.Code);
            }
        }
    }
}