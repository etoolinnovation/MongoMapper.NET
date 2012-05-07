using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


using NUnit.Framework;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using EtoolTech.MongoDB.Mapper.Configuration;

    [TestFixture()]
    public class FindTest
    {

        [Test()]
        public void TestFindByPk()
        {                    
            //Llenamos datos
            Helper.DropAllCollections();

            ConfigManager.OutConsole = true;

            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            //Insert de personas
            Person p = new Person
            {                
                Name = "Pepito Perez",
                Age = 35,
                BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                Married = true,
                Country = "ES",
                BankBalance = decimal.Parse("3500,00")
            };

            p.Childs.Add(new Child() { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });
            p.Childs.Add(new Child() { ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" });

            p.Save<Person>();

            p = new Person
            {                
                Name = "Juanito Sanchez",
                Age = 25,
                BirthDate = DateTime.Now.AddDays(52).AddYears(-38),
                Married = true,
                Country = "ES",
                BankBalance = decimal.Parse("1500,00")
            };

            p.Childs.Add(new Child() { ID = 1, Age = 5, BirthDate = DateTime.Now.AddDays(7).AddYears(-5), Name = "Toni Sanchez" });

            p.Save<Person>();

            p = new Person
            {                
                Name = "Andres Perez",
                Age = 25,
                BirthDate = DateTime.Now.AddDays(25).AddYears(-25),
                Married = false,
                Country = "ES",
                BankBalance = decimal.Parse("500,00")
            };


            p.Save<Person>();


            p = new Person
            {                
                Name = "Marta Serrano",
                Age = 28,
                BirthDate = DateTime.Now.AddDays(28).AddYears(-28),
                Married = false,
                Country = "ES",
                BankBalance = decimal.Parse("9500,00")
            };

            p.Childs.Add(new Child() { ID = 1, Age = 2, BirthDate = DateTime.Now.AddDays(2).AddYears(-2), Name = "Toni Serrano" });
            p.Save<Person>();

            p = new Person
            {                
                Name = "Jonh Smith",
                Age = 21,
                BirthDate = DateTime.Now.AddDays(21).AddYears(-21),
                Married = false,
                Country = "ES",
                BankBalance = decimal.Parse("100,00")
            };

            p.Save<Person>();


            List<Person> plist = Person.AllAsList<Person>();

            Person p2 = Person.FindByKey<Person>(plist[0].MongoMapper_Id);
            p2.Name = "FindBYKey Name";
            p2.Save<Person>();

            plist = Person.FindAsList<Person>("_id", p2.MongoMapper_Id);

            Assert.AreEqual(plist.Count,1);
            Assert.AreEqual(plist[0].Name, "FindBYKey Name");

        }


        [Test()]
        public void TestFindAnddOr()
        {
            //Llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            ConfigManager.OutConsole = true;

            List<Country> Countries = Country.FindAsList<Country>(Query.Or(MongoQuery.Eq((Country c) => c.Code, "ES"), Query.EQ("Code", "UK")));
            Assert.AreEqual(Countries.Count, 2);

            List<Person> Persons = Person.FindAsList<Person>(Query.And(MongoQuery.Eq(((Person p) => p.Age ) , 25), Query.EQ("Country", "ES")));
            Assert.AreEqual(Persons.Count, 2);

            foreach (Person p in Persons)
            {
                Assert.AreEqual(p.Age, 25);
                Assert.AreEqual(p.Country, "ES");
            }


        }


        [Test()]
        public void TestMongoCursor()
        {
            //llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            //TODO: Falla el OutPut cuando ya hemos pedido el cursor ver como hacerlo
            //A MongoCursor object cannot be modified once it has been frozen
            ConfigManager.OutConsole = false;

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
