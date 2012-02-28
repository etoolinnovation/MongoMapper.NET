using NUnit.Framework;
using System;
using System.Collections.Generic;

using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver.Builders;

using System.Threading.Tasks;


namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
	[TestFixture()]
	public class InsertModifyDeleteTest
	{
		[Test()]
		public void TestInsert()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country {Code = "es", Name = "España"};
            try
            {
                c.Save<Country>();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidatePropertyException)); 
                c.Code = "ES";
                c.Save<Country>();
            }

            c = new Country {Code = "UK", Name = "Reino Unido"};
            c.Save<Country>();

            c = new Country { Code = "UK", Name = "Reino Unido" };
            try
            {
                c.Save<Country>();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(),typeof(DuplicateKeyException));                
            }
            

            c = new Country {Code = "US", Name = "Estados Unidos"};
            c.Save<Country>();

            List<Country> Countries = Country.FindAsList<Country>("Code", "ES");
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsList<Country>("Code", "UK");
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.FindAsList<Country>("Code", "US");
            Assert.AreEqual(Countries.Count, 1);

            Countries = Country.AllAsList<Country>();
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
                Country = "US",
                BankBalance = decimal.Parse("100,00")
            };

            p.Save<Person>();

            List<Person> persons = new List<Person>();
            persons.MongoFind();

            Assert.AreEqual(persons.Count,5);


        }


        [Test()]
        public void TestParallelMultiInsert()
        {
            Helper.DropAllCollections();

            Parallel.For(0, 1000, i =>
            {
                Country c = new Country { Code = i.ToString(), Name = String.Format("Nombre {0}", i) };
                c.Save<Country>();
           }
            );

            Assert.AreEqual(1000, MongoMapper.FindAsCursor<Country>().Size());
        }

		
		[Test()]
		public void TestMultiInsert()
		{
			Helper.DropAllCollections();
			
			for(int i=0; i<1000; i++)
            {
				Country c = new Country { Code = i.ToString(), Name = String.Format("Nombre {0}",i) };
            	c.Save<Country>();

                Assert.AreEqual(i+1, MongoMapper.FindAsCursor<Country>().Size());
			}			

            Assert.AreEqual(1000, MongoMapper.FindAsCursor<Country>().Size());
		}

		[Test]
        public void TestUdpate()
        {
            Helper.DropAllCollections();
            
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

        [Test]
        public void TestDelete()
        {
            Helper.DropAllCollections();
            
            Country c = new Country { Code = "NL", Name = "Holanda" };
            c.Save<Country>();

            List<Country> Countries = Country.FindAsList<Country>("Code","NL");
            Assert.AreEqual(Countries.Count, 1);

            foreach (Country country in Countries)
            {
                country.Delete<Country>();
            }

            //TODO: Pruebas Replica Set
            //System.Threading.Thread.Sleep(5000);

            Countries = Country.FindAsList<Country>("Code", "NL");
            Assert.AreEqual(0, Countries.Count);
        }

        [Test]
        public void TestServerUdpate()
        {
            Helper.DropAllCollections();

            //Insert de Paises
            Country c = new Country {Code = "ES", Name = "España"};
            c.Save<Country>();
            c.ServerUpdate<Country>(Update.Set("Name", "España 22"));            

            Assert.AreEqual(c.Name, "España 22");

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
            p.Save<Person>();

            p.ServerUpdate<Person>(Update.PushWrapped("Childs", new Child() { ID = 2, Age = 3, BirthDate = DateTime.Now.AddDays(57).AddYears(-17), Name = "Laura Perez" }));

            Assert.AreEqual(p.Childs.Count,2);
            Assert.AreEqual(p.Childs[1].Name,"Laura Perez");


        }
	}
}

