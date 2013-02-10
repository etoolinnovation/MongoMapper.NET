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
        public void Test()
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

            Assert.AreEqual(1, col.Count);
            Assert.AreEqual(3, col.Total);

            col = new MongoMapperCollection<Country>();
            col.Find().SetLimit(3);

            Assert.AreEqual(3, col.Count);

            Assert.AreEqual("NL",col.First().Code);
            col.Find(Query<Country>.EQ(C => C.Code, "NL"));

            Assert.AreEqual("NL",col.First().Code);
            Assert.AreEqual("NL", col.Last().Code);

        }
    }
}