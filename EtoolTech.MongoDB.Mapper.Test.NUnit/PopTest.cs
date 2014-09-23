using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class PopTest
    {

        [Test]
        public void TestPop()
        {

            Helper.DropAllCollections();

            var c = new Country { Code = "PT", Name = "Portugal" };
            c.Save();
            c = new Country { Code = "ES", Name = "España"};
            c.Save();
            c = new Country { Code = "UK", Name = "Reino Unido"};
            c.Save();                     
            c = new Country { Code = "US", Name = "Estados Unidos"};
            c.Save();

            var countries = new CountryCollection();

            countries.Find();

            Assert.AreEqual(4 , countries.Count);

            c = countries.Pop();

            Assert.AreEqual(c.Code, "PT");

            Assert.AreEqual(3, countries.Count);

            c = countries.Pop();

            Assert.AreEqual(c.Code, "ES");

            Assert.AreEqual(2, countries.Count);

            c = countries.Pop();

            Assert.AreEqual(c.Code, "UK");

            Assert.AreEqual(1, countries.Count);

            c = countries.Pop();

            Assert.AreEqual(c.Code, "US");

            Assert.AreEqual(0, countries.Count);

            c = countries.Pop();

            Assert.IsNull(c);


        }


        public void TestPopCustomSort()
        {

            Helper.DropAllCollections();

            var c = new Country { Code = "A", Name = "A" };
            c.Save();
            c = new Country { Code = "B", Name = "B" };
            c.Save();
            c = new Country { Code = "C", Name = "C" };
            c.Save();
            c = new Country { Code = "D", Name = "D" };
            c.Save();

            var countries = new CountryCollection();

            countries.Find();

            Assert.AreEqual(4, countries.Count);

            c = countries.Pop(Query.Null, SortBy<Country>.Ascending(C=>C.Code));

            Assert.AreEqual(c.Code, "A");

            Assert.AreEqual(3, countries.Count);

            c = countries.Pop(Query.Null, SortBy<Country>.Ascending(C=>C.Code));

            Assert.AreEqual(c.Code, "B");

            Assert.AreEqual(2, countries.Count);

            c = countries.Pop(Query.Null, SortBy<Country>.Ascending(C=>C.Code));

            Assert.AreEqual(c.Code, "C");

            Assert.AreEqual(1, countries.Count);

            c = countries.Pop(Query.Null, SortBy<Country>.Ascending(C=>C.Code));

            Assert.AreEqual(c.Code, "D");

            Assert.AreEqual(0, countries.Count);

            c = countries.Pop(Query.Null, SortBy<Country>.Ascending(C=>C.Code));

            Assert.IsNull(c);


        }


        public void TestPopCustomSortCustomQuery()
        {

            Helper.DropAllCollections();

            var c = new Country { Code = "A", Name = "1" };
            c.Save();
            c = new Country { Code = "B", Name = "1" };
            c.Save();
            c = new Country { Code = "C", Name = "C" };
            c.Save();
            c = new Country { Code = "D", Name = "1" };
            c.Save();

            var countries = new CountryCollection();

            countries.Find();

            Assert.AreEqual(4, countries.Count);

            c = countries.Pop(MongoQuery<Country>.Eq(C => C.Name, "1"), SortBy<Country>.Descending(C=>C.Code));

            Assert.AreEqual(c.Code, "D");

            Assert.AreEqual(3, countries.Count);

            c = countries.Pop(MongoQuery<Country>.Eq(C => C.Name, "1"), SortBy<Country>.Descending(C => C.Code));

            Assert.AreEqual(c.Code, "B");

            Assert.AreEqual(2, countries.Count);

            c = countries.Pop(MongoQuery<Country>.Eq(C => C.Name, "1"), SortBy<Country>.Descending(C => C.Code));

            Assert.AreEqual(c.Code, "A");

            Assert.AreEqual(1, countries.Count);

            c = countries.Pop(MongoQuery<Country>.Eq(C => C.Name, "1"), SortBy<Country>.Descending(C => C.Code));

            Assert.IsNull(c);


        }
    }
}
