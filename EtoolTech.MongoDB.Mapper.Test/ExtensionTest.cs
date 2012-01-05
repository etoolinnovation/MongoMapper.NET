using System;
using System.Collections.Generic;
using EtoolTech.MongoDB.Mapper.Test.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void TestExtensions()
        {
            Helper.Db.Drop();

            //Insert de Paises
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save<Country>();
            c = new Country { Code = "US", Name = "Estados Unidos" };
            c.Save<Country>();

            List<Country> countries = new List<Country>();
            countries.MongoFind();
            Assert.AreEqual(countries.Count,3);

            countries.MongoFind(MongoQuery.Eq((Country co) => co.Code,"ES"));
            Assert.AreEqual(countries.Count, 1);
            Assert.AreEqual(countries[0].Code, "ES");

            countries.MongoFind(Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
            Assert.AreEqual(countries.Count, 2);

            Country country = new Country();
            //TODO: Pendiente de ver como no tener que igualarlo
            country = country.FindByKey("ES");
            Assert.AreEqual(country.Code,"ES");

            List<string> strings = new List<string>();
            try
            {
                strings.MongoFind();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(NotSupportedException));
            }

            string s = "s";
            try
            {
                s.FindByKey("");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(NotSupportedException));
            }

        }
    }
}
