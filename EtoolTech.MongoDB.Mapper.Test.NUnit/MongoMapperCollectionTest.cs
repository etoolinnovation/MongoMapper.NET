using System;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class MongoMapperCollectionTest
    {

        [Test]
        public void Cursor()
        {

            Helper.DropAllCollections();

            for (int i = 0; i < 50; i++)
            {
                Country c = new Country();
                c.Code = "C" + i.ToString();
                c.Name = "Name" + i.ToString();
                c.Save();
            }

            CountryCollection col = new CountryCollection();
            col.Find().Sort(col.Sort.Descending("Code"));
       
            Console.WriteLine(col.Count);

            foreach (var c in col)
            {
                Console.WriteLine(c.Code);
            }
        }


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

            var col = new CountryCollection {FromPrimary = false};
            col.Find();

            foreach (Country c in col)
            {
                Console.WriteLine(c.Name);
            }

            col.Find().Limit(1);

            //Console.WriteLine(col.Cursor.Explain().ToJson());

            Assert.AreEqual(1, col.Count);
            Assert.AreEqual(3, col.Total);
       
            col = new CountryCollection {FromPrimary = true};
            col.Find().Limit(3).Sort(col.Sort.Ascending(C=>C.Name));
        
            Assert.AreEqual(3, col.Count);
            Assert.AreEqual("ES", col.First().Code);

            col.Find(MongoQuery<Country>.Eq(C => C.Code, "NL"));

            Assert.AreEqual("NL", col.First().Code);
            //TODO: No esta implementado el last Assert.AreEqual("NL", col.Last().Code);

       

        }
    }
}