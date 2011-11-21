using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Core;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestInsert()
        {
            Helper.Db.Drop();
            //Insert de Paises
            Country c = new Country {Code = "es", Name = "España"};
            try
            {
                c.Save<Country>();
            }
            catch (ValidatePropertyException ex)
            {
                Assert.AreEqual(ex.Message, "Error Validating Property Code: es must be ES");
                c.Code = "ES";
                c.Save<Country>();
            }

            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save<Country>();
            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save<Country>();

            List<Country> Countries = Country.FindAsList<Country>("Code", "ES");
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsList<Country>("Code", "UK");
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsList<Country>("Code", "US");
            Assert.AreEqual(Countries.Count, 1);


            //Insert de personas
            Person p = new Person
            {
                Id = 1,
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
                Id = 2,
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
                Id = 3,
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
                Id = 4,
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
                Id = 5,
                Name = "Jonh Smith",
                Age = 21,
                BirthDate = DateTime.Now.AddDays(21).AddYears(-21),
                Married = false,
                Country = "US",
                BankBalance = decimal.Parse("100,00")
            };

            p.Save<Person>();

            for(int i=0; i<5; i++)
            {
                int I = i + 1;
                List<Person> Persons = Person.FindAsList<Person>("Id", I);
                Assert.AreEqual(Persons.Count,1);
            }
        }

        [TestMethod]
        public void TestFindAnddOr()
        {
            //Llenamos datos
            TestInsert();
                        

            List<Country> Countries = Country.FindAsList<Country>(Query.Or(Query.EQ("Code", "ES"),Query.EQ("Code", "UK")));
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
            TestInsert();

            List<Country> Countries = Country.FindAsCursor<Country>().ToList();
            Assert.AreEqual(Countries.Count, 3);

            Countries = Country.FindAsCursor<Country>().SetSkip(2).ToList();
            Assert.AreEqual(Countries.Count, 1);


            Countries = Country.FindAsCursor<Country>().SetLimit(1).ToList();
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsCursor<Country>(Query.EQ("Code","ES")).SetFields(Fields.Include("Code")).ToList();
            Assert.AreEqual(Countries.Count, 1);
            Assert.AreEqual(Countries.First().Name, null);

        }
        
        [TestMethod]
        public void TestUdpate()
        {            
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            Country c2 = Country.FindByKey<Country>("ES");
            c2.Name = "España Up";
            c2.Save<Country>();

            Country c3 = Country.FindByKey<Country>("ES");

            Assert.AreEqual(c3.Name, "España Up");

            List<Country> Countries = Country.FindAsList<Country>("Code", "ES");
            Assert.AreEqual(Countries.Count, 1);
        }

        [TestMethod]
        public void TestDelete()
        {            
            Country c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            List<Country> Countries = Country.FindAsList<Country>("Code","NL");
            Assert.AreEqual(Countries.Count, 1);

            foreach (Country country in Countries)
            {
                country.Delete<Country>();
            }

            //Countries = Country.FindAsList<Country>(x => x.Code == "NL");
            //Assert.AreEqual(Countries.Count, 0);
        }

        [TestMethod]
        public void TestRelations()
        {
            Country c = new Country {Code = "ES", Name = "España"};
            c.Save<Country>();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save<Country>();

            Person p = new Person
                            {
                                Id = 1,
                                Name = "Pepito Perez",
                                Age = 35,
                                BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                                Married = true,
                                Country = "XXXXX",
                                BankBalance = decimal.Parse("3500,00")
                            };

            p.Childs.Add(new Child()
                                {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Childs.Add(new Child()
                                {ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"});


            bool ValidateRelationExceptionThrow = false;
            try
            {
                p.Save<Person>();
            }
            catch (ValidateUpRelationException)
            {
                ValidateRelationExceptionThrow = true;
                p.Country = "ES";
                p.Save<Person>();
            }

            Assert.AreEqual(ValidateRelationExceptionThrow, true);
            ValidateRelationExceptionThrow = false;

            c = Country.FindByKey<Country>("ES");
            try
            {
                c.Delete<Country>();
            }
            catch (ValidateDownRelationException)
            {
                ValidateRelationExceptionThrow = true;
                List<Person> Persons = c.GetRelation<Person>("Person,Country");
                foreach (Person p2 in Persons)
                {
                    p2.Country = "UK";
                    p2.Save<Person>();
                }
                c.Delete<Person>();
            }

            Assert.AreEqual(ValidateRelationExceptionThrow, true);

            c = Country.FindByKey<Country>("UK");

            List<Person> PersonasEnUK = c.GetRelation<Person>("Person,Country");
            foreach (Person PersonInUK in PersonasEnUK)
            {
                Assert.AreEqual(PersonInUK.Country,"UK");
                Assert.AreEqual(PersonInUK.GetRelation<Country>("Country,Code").First().Code, "UK");
            }
                       
      
        }

        [TestMethod]
        public void TestOriginalValue()
        {

            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            Person p = new Person
            {
                Id = 1,
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
            
            p.Name = "Juan Sin Miedo";

            object OriginalName = p.GetOriginalValue<Person>("Name");

            Assert.AreEqual(OriginalName.ToString(), "Pepito Perez");
       

        }

        [TestMethod]
        public void TestEvents()
        {
            Country c = new Country { Code = "FR", Name = "España" };
            c.OnBeforeInsert += (s, e) => { ((Country) s).Name = "Francia"; };
            c.OnBeforeModify += (s, e) => { ((Country)s).Name = "Francia"; };
            c.Save<Country>();

            Country c3 = Country.FindByKey<Country>("FR");

            Assert.AreEqual(c3.Name, "Francia");
            
        }


  
    }
}