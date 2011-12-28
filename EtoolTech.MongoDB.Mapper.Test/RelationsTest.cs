using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Test.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class RelationsTest
    {
        [TestMethod]
        public void TestRelations()
        {
            Helper.Db.Drop();

            Country c = new Country { Code = "ES", Name = "España" };
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

            p.Childs.Add(new Child() { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });
            p.Childs.Add(new Child() { ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" });


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
                Assert.AreEqual(PersonInUK.Country, "UK");
                Assert.AreEqual(PersonInUK.GetRelation<Country>("Country,Code").First().Code, "UK");
            }


        }

    }
}
