using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Test.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class FindTest
    {
        [TestMethod]
        public void TestFindAnddOr()
        {
            //Llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();


            List<Country> Countries = Country.FindAsList<Country>(Query.Or(Query.EQ("Code", "ES"), Query.EQ("Code", "UK")));
            Assert.AreEqual(Countries.Count, 2);

            List<Person> Persons = Person.FindAsList<Person>(Query.And(Query.EQ("Age", 25), Query.EQ("Country", "ES")));
            Assert.AreEqual(Persons.Count, 2);

            foreach (Person p in Persons)
            {
                Assert.AreEqual(p.Age, 25);
                Assert.AreEqual(p.Country, "ES");
            }


        }


        [TestMethod]
        public void TestMongoCursor()
        {
            //llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            List<Country> Countries = Country.FindAsCursor<Country>().ToList();
            Assert.AreEqual(Countries.Count, 3);

            Countries = Country.FindAsCursor<Country>().SetSkip(2).ToList();
            Assert.AreEqual(Countries.Count, 1);


            Countries = Country.FindAsCursor<Country>().SetLimit(1).ToList();
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsCursor<Country>(Query.EQ("Code", "ES")).SetFields(Fields.Include("Code")).ToList();
            Assert.AreEqual(Countries.Count, 1);
            Assert.AreEqual(Countries.First().Name, null);

        }


    }
}
