using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class InsertModifyDeleteTest
    {
       
        [Test]
        public void TestDelete()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "NL", Name = "Holanda"};
            c.Save();

            var countries = new CountryCollection();
            countries.Find(X=>X.Code, "NL");
            Assert.AreEqual(1,countries.Count);

            foreach (Country country in countries)
            {
                country.Delete();
            }

            //TODO: Pruebas Replica Set
            //System.Threading.Thread.Sleep(5000);

            countries.Find(X=>X.Code, "NL");
            Assert.AreEqual(0, countries.Count);
        }

        [Test]
        public void TestInsert()
        {
            Helper.DropAllCollections();


            //Paris
            var geoArea = new GeoArea();
            geoArea.type = "Polygon";            
            var coordinates = new List<double[]>();
            coordinates.Add(new double[] { 48.979766324449706, 2.098388671875 });
            coordinates.Add(new double[] { 48.972555195463336, 2.5982666015625 });
            coordinates.Add(new double[] { 48.683254235765325, 2.603759765625 });
            coordinates.Add(new double[] { 48.66874533279169, 2.120361328125 });
            coordinates.Add(new double[] { 48.979766324449706, 2.098388671875 });
            geoArea.coordinates = new [] {coordinates.ToArray()};

            //Insert de Paises
            var c = new Country {Code = "es", Name = "España", Area = geoArea};
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

            c = new Country { Code = "UK", Name = "Reino Unido", Area = geoArea};
            c.Save();

            c = new Country { Code = "UK", Name = "Reino Unido", Area = geoArea };
            try
            {
                c.Save();
                Assert.Fail();
            }
            catch (DuplicateKeyException ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (DuplicateKeyException));
            }

            c = new Country { Code = "US", Name = "Estados Unidos", Area = geoArea };
            c.Save();

            var countries = new CountryCollection();

            countries.Find(x=>x.Code, "ES");
            Assert.AreEqual(countries.Count, 1);

            countries.Find(x=>x.Code, "UK");
            Assert.AreEqual(countries.Count, 1);

            countries.Find(x=>x.Code, "US");
            Assert.AreEqual(countries.Count, 1);

            countries.Find();
            Assert.AreEqual(countries.Count, 3);

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
                    BankBalance = decimal.Parse("10000,00")
                };

            p.Save();

            var persons = new List<Person>();
            persons.MongoFind();

            Assert.AreEqual(persons.Count, 5);
        }

        [Test]
        public void TestMultiInsert()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                c.Save();

                Assert.AreEqual(i + 1,CountryCollection.Instance.Find().CountAsync().Result);
            }

            Assert.AreEqual(100, CountryCollection.Instance.Find().CountAsync().Result);
        }

        [Test]
        public void TestParallelMultiInsert()
        {
            Helper.DropAllCollections();

            Parallel.For(
                0,
                100,
                i =>
                    {
                        var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                        c.Save();
                    });

            Assert.AreEqual(100, CountryCollection.Instance.Find().CountAsync().Result);
        }

        [Test]
        public void TestServerDelete()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                c.Save();

                Assert.AreEqual(i + 1, CountryCollection.Instance.Find().CountAsync().Result);
            }

            MongoMapper<Country>.ServerDelete(MongoQuery<Country>.Eq(c=>c.Code, "0"));

            Assert.AreEqual(99, CountryCollection.Instance.Find().CountAsync().Result);
        }

        [Test]
        public void TestServerUdpate()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c.ServerUpdate(c.Update.Set(MongoMapperHelper.ConvertFieldName("Country","Name"), "España 22"));

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

        [Test]
        public void TestUdpate()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            var c2 = MongoMapper<Country>.FindByKey("ES");
            c2.Name = "España Up";
            c2.Save();

            var c3 = MongoMapper<Country>.FindByKey("ES");

            Assert.AreEqual(c3.Name, "España Up");

            var Countries = CountryCollection.Instance;
            Countries.Find(x=>x.Code, "ES");
            Assert.AreEqual(Countries.Count, 1);
        }
    }
}