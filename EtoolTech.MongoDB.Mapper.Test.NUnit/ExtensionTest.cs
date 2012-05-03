using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Exceptions;
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
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save();
            c = new Country { Code = "US", Name = "Estados Unidos" };
            c.Save();

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
            Helper.DropAllCollections();
            
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

        [Test()]
        public void TestSave()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save();

            Country country = new Country();
            country.FillByKey("ES");
            Assert.AreEqual(country.Code, "ES"); 
        }

        [Test()]
        public void TestDelete()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save();

            c.FillByKey("ES");
            c.Delete();

            //TODO: Pruebas Replica Set
            //System.Threading.Thread.Sleep(5000);

            List<Country> country = new List<Country>();
            country.MongoFind();
            Assert.AreEqual(0, country.Count);
        }


        [Test()]
        public void TestInsert()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country { Code = "es", Name = "España" };
            try
            {
                c.Save();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidatePropertyException));
                c.Code = "ES";
                c.Save();
            }

            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save();

            c = new Country { Code = "UK", Name = "Reino Unido" };
            try
            {
                c.Save();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(DuplicateKeyException));
            }



            c = new Country { Code = "US", Name = "Estados Unidos" };
            c.Save();

            List<Country> Countries = new List<Country>();
            Countries.MongoFind("Code", "ES");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind("Code", "UK");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind("Code", "US");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind();
            Assert.AreEqual(Countries.Count, 3);

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

            p.Save();

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

            p.Save();

            p = new Person
            {
                Name = "Andres Perez",
                Age = 25,
                BirthDate = DateTime.Now.AddDays(25).AddYears(-25),
                Married = false,
                Country = "ES",
                BankBalance = decimal.Parse("500,00")
            };


            p.Save();


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
            p.Save();

            p = new Person
            {
                Name = "Jonh Smith",
                Age = 21,
                BirthDate = DateTime.Now.AddDays(21).AddYears(-21),
                Married = false,
                Country = "US",
                BankBalance = decimal.Parse("100,00")
            };

            p.Save();

            List<Person> persons = new List<Person>();
            persons.MongoFind();            
            Assert.AreEqual(persons.Count, 5);

            persons.MongoFind("Childs.Age", 2);
            Assert.AreEqual(persons.Count, 1);


        }
 	
		
		public void TestPerfFillByKeyNormalVsExtensionMethod()
        {
            
            Helper.DropAllCollections();

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
                Country.FindByKey<Country>("ES");                
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:27.1441065
        }

        public void TestPerfMongoFindNormalVsExtensionMethods()
        {
            Helper.DropAllCollections();

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
                Country.FindAsList<Country>(Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:10.1821050

        }
    }
}
