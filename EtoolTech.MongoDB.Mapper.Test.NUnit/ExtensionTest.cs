using System;
using System.Collections.Generic;

using NUnit.Framework;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System.Diagnostics;

    [TestFixture()]
    public class ExtensionTest
    {
        [Test()]
        public void TestCollectionExtensions()
        {
            Helper.DropAllDb();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save<Country>();
            c = new Country { Code = "US", Name = "Estados Unidos" };
            c.Save<Country>();

            List<Country> countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(countries.Count, 3);

            countries.MongoFind(MongoQuery.Eq((Country co) => co.Code, "ES"));
            Assert.AreEqual(countries.Count, 1);
            Assert.AreEqual(countries[0].Code, "ES");

            countries.MongoFind(
                Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
            Assert.AreEqual(countries.Count, 2);

         
            List<string> strings = new List<string>();
            try
            {
                strings.MongoFind();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(NotSupportedException));
            }

        }

        [Test()]
        public void TestFillByKeyExtension()
        {
            Helper.DropAllDb();
            
            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
         
            Country country = new Country();
            country.FillByKey("ES");
            Assert.AreEqual(country.Code, "ES");

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

            long id = p.MongoMapper_Id;            

            p = new Person();
            p.FillByKey(id);
            

            string s = "";
            try
            {
                s.FillByKey(null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(NotSupportedException));
            }
        }

        public void TestPerfFillByKeyNormalVsExtensionMethod()
        {
            
            Helper.DropAllDb();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            var timer = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                Country country = new Country();
                country.FillByKey("ES");
            }
            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para ExtensionMethod: {0}", timer.Elapsed));
            //Elapsed para ExtensionMethod: 00:04:38.5479462

            timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                Country country = Country.FindByKey<Country>("ES");                
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:27.1441065
        }

        public void TestPerfMongoFindNormalVsExtensionMethods()
        {
            Helper.DropAllDb();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save<Country>();
            c = new Country { Code = "US", Name = "Estados Unidos" };
            c.Save<Country>();

            var timer = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
               List<Country> countries = new List<Country>();
               countries.MongoFind(
               Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
            }
            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para ExtensionMethod: {0}", timer.Elapsed));
            //Elapsed para ExtensionMethod: 00:04:29.8042031

            timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                List<Country> countries = Country.FindAsList<Country>(Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:10.1821050

        }
    }
}
