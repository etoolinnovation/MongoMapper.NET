using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Core;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using EtoolTech.MongoDB.Mapper.Test.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test
{
    /// <summary>
    /// Summary description for InsertModifyDeleteTest
    /// </summary>
    [TestClass]
    public class InsertModifyDeleteTest
    {
        public InsertModifyDeleteTest()
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

            Countries = Country.AllAsList<Country>();

            //foreach (Country c2 in Countries)
            //{
            //    Country c3 = c2.GetOriginalObject<Country>();
            //}


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
       
    }
}