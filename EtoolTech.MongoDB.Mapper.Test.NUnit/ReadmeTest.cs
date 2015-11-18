using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class ReadmeTest
    {

        [Test]
        public void Test()
        {
            Helper.DropAllCollections();

            ConfigManager.Out = Console.Out;

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

            using (var t = new MongoMapperTransaction())
            {
                var c2 = new Country {Code = "US", Name = "Francia"};
                c2.OnBeforeInsert += (s, e) => { ((Country) s).Name = "Estados Unidos"; };
                c2.Save();

                t.Commit();
            }

            var c3 = new Country();
            c3.FillByKey("US");
            Assert.AreEqual(c3.Name, "Estados Unidos");

            if (!c3.IsLastVersion())
                c3.FillFromLastVersion();

            var countries = new CountryCollection();
            countries.Find();
            Assert.AreEqual(countries.Count, 3);

            countries.Find().Limit(2).Sort(Builders<Country>.Sort.Ascending(C=>C.Name));
            Assert.AreEqual(countries.Count, 2);
            Assert.AreEqual(countries.Total, 3);

            countries.Find(
                Builders<Country>.Filter.Or(MongoQuery<Country>.Eq(co => co.Code, "ES"), MongoQuery<Country>.Eq(co => co.Code, "UK")));
            Assert.AreEqual(countries.Count, 2);

            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "XXXXX",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});

            try
            {
                p.Save();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof (ValidateUpRelationException));
                p.Country = "ES";
                p.Save();
            }

            p.ServerUpdate(
                Builders<Person>.Update.Push(
                    MongoMapperHelper.ConvertFieldName("Person","Childs"),
                    new Child {ID = 2, Age = 2, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"}));

            var persons = new List<Person>();

            persons.MongoFind();
            
            persons.MongoFind("Childs.Age", 2);
            Assert.AreEqual(1, persons.Count);
        }
    }
}