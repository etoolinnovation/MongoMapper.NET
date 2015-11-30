using System;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class FindTest
    {
      
        [Test]
        public void TestFindAnddOr()
        {
            var Countries = CountryCollection.Instance;
            var Persons = MongoMapperCollection<Person>.Instance;

            Countries.Find(MongoQuery<Country>.Eq(c => c.Code, BsonNull.Value));

            //Llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            ConfigManager.Out = Console.Out;


            

            Countries.Find(
                    Countries.Filter.Or(MongoQuery<Country>.Eq(c=>c.Code, "ES"), MongoQuery<Country>.Eq(c=>c.Code, "UK")));
            Assert.AreEqual(Countries.Count, 2);

            Persons.Find(
                    Persons.Filter.And(MongoQuery<Person>.Eq(p => p.Age, 25), MongoQuery<Person>.Eq(p => p.Country, "ES")));
            Assert.AreEqual(Persons.Count, 2);

            Persons = new MongoMapperCollection<Person>();
            Persons.Find(
                    Persons.Filter.And(MongoQuery<Person>.Eq(p => p.Age, 35), MongoQuery<Person>.Eq(p => p.Country, "%")));
            Assert.AreEqual(1, Persons.Count);

            Persons = new MongoMapperCollection<Person>();
          
            Persons.Find(MongoQuery<Person>.Eq(p => p.Name, "%perez%"));
            Assert.AreEqual(2, Persons.Count);


            Persons.Find(MongoQuery<Person>.Eq(p => p.Name, "j%"));
            Assert.AreEqual(2, Persons.Count);

            Persons.Find(MongoQuery<Person>.Eq(p => p.Name, "%Z"));
            Assert.AreEqual(3, Persons.Count);

            
            Persons.Find(MongoQuery<Person>.Eq(p => p.Married, false));
            Assert.AreEqual(3, Persons.Count);

            Persons.Find(Persons.Filter.And(MongoQuery<Person>.Eq(p => p.Age, 25), MongoQuery<Person>.Eq(p => p.Country, "ES")));
            Assert.AreEqual(2, Persons.Count);


            foreach (Person p in Persons)
            {
                Assert.AreEqual(p.Age, 25);
                Assert.AreEqual(p.Country, "ES");
            }

            Persons.Find(Persons.Filter.And(MongoQuery<Person>.Eq(p => p.Age, 25), MongoQuery<Person>.Eq(p => p.Country, "US")));
            Assert.AreEqual(0, Persons.Count);

            Persons.Find(MongoQuery<Person>.Gt(p => p.Age, 25));
            Assert.AreEqual(2, Persons.Count);


            Persons.Find(MongoQuery<Person>.Gte(p => p.Age, 25));
            Assert.AreEqual(4, Persons.Count);

            Persons.Find(MongoQuery<Person>.Lt(p => p.Age, 25));
            Assert.AreEqual(1, Persons.Count);

            Persons.Find(MongoQuery<Person>.Lte(p => p.Age, 25));
            Assert.AreEqual(3, Persons.Count);


        }

        [Test]
        public void TestFindByPk()
        {
            //Llenamos datos
            Helper.DropAllCollections();

            ConfigManager.Out = Console.Out;

            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

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
                    Country = "ES",
                    BankBalance = decimal.Parse("100,00")
                };

            p.Save();

            List<Person> plist = MongoMapperCollection<Person>.Instance.Find().ToListAsync().Result;

            var p2 = new Person();
            p2.FillByKey(plist[0].m_id);
            p2.Name = "FindBYKey Name";
            p2.Save();

            
            var Persons = MongoMapperCollection<Person>.Instance;

            plist = Persons.Find(x=>x.m_id, p2.m_id).ToListAsync().Result;

            Assert.AreEqual(1, plist.Count);
            Assert.AreEqual(plist[0].Name, "FindBYKey Name");

            c = new Country();

            //Assert.Throws(typeof(FindByKeyNotFoundException), c.FillByKey("XXXXXX"));
            
        }

        [Test]
        public void TestMongoCursor()
        {
            //llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            //TODO: Falla el OutPut cuando ya hemos pedido el cursor ver como hacerlo
            //A MongoCursor object cannot be modified once it has been frozen
            ConfigManager.Out = null;


            var Countries = CountryCollection.Instance;

            Countries.Find();
            Assert.AreEqual(Countries.Count, 3);

            Countries.Find().Skip(2);
            Assert.AreEqual(Countries.Count, 1);

            Countries.Find().Limit(1);
            Assert.AreEqual(Countries.Count, 1);

            Countries.Find(MongoQuery<Country>.Eq(c=>c.Code, "ES")).Project(Countries.Project.Include(C=>C.Code));
            Assert.AreEqual(Countries.Count, 1);
            //var c2 = Countries.First();
            //TODO Projection no esta funcionando, devuelve Name tambien Assert.AreEqual(c2.Name, null);
        }
    }
}