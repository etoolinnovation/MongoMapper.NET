namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Linq;

    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Exceptions;

    using global::NUnit.Framework;

    [TestFixture]
    public class RelationsTest
    {
        //private MongoTestServer _mongoTestServer;

        //[TestFixtureSetUp]
        //public void Init()
        //{
        //    MongoTestServer.SetMongodPtah(@"mongod\");
        //    this._mongoTestServer = MongoTestServer.Start(27017);
        //    ConfigManager.OverrideConnectionString(this._mongoTestServer.ConnectionString);
        //}

        //[TestFixtureTearDown]
        //public void Dispose()
        //{
        //    this._mongoTestServer.Dispose();
        //}

        #region Public Methods

        [Test]
        public void TestRelations()
        {
            Helper.DropAllCollections();

            var c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
            c = new Country { Code = "UK", Name = "Reino Unido" };
            c.Save<Country>();

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
                new Child { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });
            p.Childs.Add(
                new Child { ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" });

            try
            {
                p.Save<Person>();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidateUpRelationException));
                p.Country = "ES";
                p.Save<Person>();
            }

            c = MongoMapper.FindByKey<Country>("ES");
            try
            {
                c.Delete<Country>();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidateDownRelationException));
                global::System.Collections.Generic.List<Person> Persons = c.GetRelation<Person>("Person,Country");
                foreach (Person p2 in Persons)
                {
                    p2.Country = "UK";
                    p2.Save<Person>();
                }
                c.Delete<Person>();
            }

            c = MongoMapper.FindByKey<Country>("UK");

            global::System.Collections.Generic.List<Person> PersonasEnUK = c.GetRelation<Person>("Person,Country");
            foreach (Person PersonInUK in PersonasEnUK)
            {
                Assert.AreEqual(PersonInUK.Country, "UK");
                Assert.AreEqual(PersonInUK.GetRelation<Country>("Country,Code").First().Code, "UK");
            }
        }

        #endregion
    }
}