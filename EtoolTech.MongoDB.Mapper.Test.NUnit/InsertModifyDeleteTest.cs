using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class InsertModifyDeleteTest
    {
        //private MongoTestServer _mongoTestServer;

        //[TestFixtureSetUp]
        //public void Init()
        //{
        //    MongoTestServer.SetMongodPtah(@"mongod\");
        //    this._mongoTestServer = MongoTestServer.Start(27017);
        //    ConfigManager.OverrideUrlString(this._mongoTestServer.ConnectionString);
        //}

        //[TestFixtureTearDown]
        //public void Dispose()
        //{
        //    this._mongoTestServer.Dispose();
        //}

        [Test]
        public void TestDelete()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "NL", Name = "Holanda"};
            c.Save();

            var Countries = new CountryCollection();
            Countries.Find(x=>x.Code, "NL");
            Assert.AreEqual(1,Countries.Count);

            foreach (Country country in Countries)
            {
                country.Delete();
            }

            //TODO: Pruebas Replica Set
            //System.Threading.Thread.Sleep(5000);

            Countries.Find(x=>x.Code, "NL");
            Assert.AreEqual(0, Countries.Count);
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (DuplicateKeyException));
            }

            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save();

            var Countries = new CountryCollection();

            Countries.Find(x=>x.Code, "ES");
            Assert.AreEqual(Countries.Count, 1);

            Countries.Find(x=>x.Code, "UK");
            Assert.AreEqual(Countries.Count, 1);

            Countries.Find(x=>x.Code, "US");
            Assert.AreEqual(Countries.Count, 1);

            Countries.Find();
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
        }

        [Test]
        public void TestMultiInsert()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                c.Save();

                Assert.AreEqual(i + 1,CountryCollection.Instance.Find().Size());
            }

            Assert.AreEqual(100, CountryCollection.Instance.Find().Size());
        }

        [Test]
        public void TestParallelMultiInsert()
        {
            Helper.DropAllCollections();

            Parallel.For(
                0,
                1000,
                i =>
                    {
                        var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                        c.Save();
                    });

            Assert.AreEqual(1000, CountryCollection.Instance.Find().Size());
        }

        [Test]
        public void TestServerDelete()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                c.Save();

                Assert.AreEqual(i + 1, CountryCollection.Instance.Find().Size());
            }

            MongoMapper.ServerDelete<Country>(Query.EQ("Code", "0"));

            Assert.AreEqual(99, CountryCollection.Instance.Find().Size());
        }

        [Test]
        public void TestServerUdpate()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            var c = new Country {Code = "ES", Name = "España"};
            c.Save();
            c.ServerUpdate(Update.Set("Name", "España 22"));

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
                Update.PushWrapped(
                    "Childs",
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

            var c2 = MongoMapper.FindByKey<Country>("ES");
            c2.Name = "España Up";
            c2.Save();

            var c3 = MongoMapper.FindByKey<Country>("ES");

            Assert.AreEqual(c3.Name, "España Up");

            var Countries = CountryCollection.Instance;
            Countries.Find(x=>x.Code, "ES");
            Assert.AreEqual(Countries.Count, 1);
        }
    }
}