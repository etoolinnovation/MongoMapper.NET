using System;
using System.Collections.Generic;
using System.Diagnostics;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class ExtensionTest
    {      

        public void TestPerfFillByKeyNormalVsExtensionMethod()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                var country = new Country();
                country.FillByKey("ES");
            }
            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para ExtensionMethod: {0}", timer.Elapsed));
            //Elapsed para ExtensionMethod: 00:04:38.5479462

            timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                MongoMapper<Country>.FindByKey("ES");
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:27.1441065
        }

        public void TestPerfMongoFindNormalVsExtensionMethods()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save();
            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save();

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                var countries = new List<Country>();
                countries.MongoFind(
                    Builders<Country>.Filter.Or(MongoQuery<Country>.Eq(co => co.Code, "ES"), MongoQuery<Country>.Eq(co => co.Code, "UK")));
            }
            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para ExtensionMethod: {0}", timer.Elapsed));
            //Elapsed para ExtensionMethod: 00:04:29.8042031

            timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                CountryCollection mongoCol = new CountryCollection();
                mongoCol.Find(
                    mongoCol.Filter.Or(MongoQuery<Country>.Eq(co=>co.Code, "ES"), MongoQuery<Country>.Eq(co => co.Code, "UK")));
                mongoCol.ToList();
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para StaticMethod: {0}", timer.Elapsed));
            //Elapsed para StaticMethod: 00:04:10.1821050
        }

        [Test]
        public void TestCollectionExtensions()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save();
            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save();

            var countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(countries.Count, 3);

            countries.MongoFind(MongoQuery<Country>.Eq(co => co.Code, "ES"));
            Assert.AreEqual(countries.Count, 1);
            Assert.AreEqual(countries[0].Code, "ES");

            countries.MongoFind(
                Builders<Country>.Filter.Or(MongoQuery<Country>.Eq(co => co.Code, "ES"), MongoQuery<Country>.Eq(co => co.Code, "UK")));
            Assert.AreEqual(countries.Count, 2);
        }

        [Test]
        public void TestDelete()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            c.FillByKey("ES");
            c.Delete();

            //TODO: Pruebas Replica Set
            //System.Threading.Thread.Sleep(5000);

            var country = new List<Country>();
            country.MongoFind();
            Assert.AreEqual(0, country.Count);
        }

        [Test]
        public void TestFillByKeyExtension()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            var country = new Country();
            country.FillByKey("ES");
            Assert.AreEqual(country.Code, "ES");

            //Insert de personas
            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "ES",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Childs.Add(
                new Child {ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"});

            p.Save();

            long id = p.m_id;

            p = new Person();
            p.FillByKey(id);
        }

        [Test]
        public void TestInsert()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "es", Name = "España"};
            try
            {
                c.Save();
                Assert.Fail();
            }
            catch (ValidatePropertyException ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (ValidatePropertyException));
                c.Code = "ES";
                c.Save();
            }

            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save();

            c = new Country {Code = "UK", Name = "Reino Unido"};
            try
            {
                c.Save();
                Assert.Fail();
            }
            catch (DuplicateKeyException ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (DuplicateKeyException));
            }

            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save();

            var Countries = new List<Country>();
            Countries.MongoFind(C=>C.Code, "ES");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind(C=>C.Code, "UK");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind(C=>C.Code, "US");
            Assert.AreEqual(Countries.Count, 1);

            Countries.MongoFind();
            Assert.AreEqual(Countries.Count, 3);

            //Insert de personas
            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "ES",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Childs.Add(
                new Child {ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"});

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

            p.Childs.Add(
                new Child {ID = 1, Age = 5, BirthDate = DateTime.Now.AddDays(7).AddYears(-5), Name = "Toni Sanchez"});

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

            p.Childs.Add(
                new Child {ID = 1, Age = 2, BirthDate = DateTime.Now.AddDays(2).AddYears(-2), Name = "Toni Serrano"});
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

            var persons = new List<Person>();
            persons.MongoFind();
            Assert.AreEqual(persons.Count, 5);

            persons.MongoFind("Childs.Age", 2);
            Assert.AreEqual(persons.Count, 1);
        }

        [Test]
        public void TestSave()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            var country = new Country();
            country.FillByKey("ES");
            Assert.AreEqual(country.Code, "ES");
        }

        [Test]
        public void TestServerUdpate()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c.ServerUpdate(c.Update.Set(MongoMapperHelper.ConvertFieldName("Country", "Name"),
                "España 22"));
                                

            Assert.AreEqual(c.Name, "España 22");

            //Insert de personas
            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "ES",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Save();

            p.ServerUpdate(
                p.Update.Push(
                    MongoMapperHelper.ConvertFieldName("Person","Childs"),
                    new Child
                        {ID = 2, Age = 3, BirthDate = DateTime.Now.AddDays(57).AddYears(-17), Name = "Laura Perez"}));

            Assert.AreEqual(p.Childs.Count, 2);
            Assert.AreEqual(p.Childs[1].Name, "Laura Perez");
        }
    }
}