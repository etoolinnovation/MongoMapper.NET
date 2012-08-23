namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Linq;

    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::MongoDB.Driver.Builders;

    using global::NUnit.Framework;

    [TestFixture]
    public class FindTest
    {
        private MongoTestServer _mongoTestServer;

        [TestFixtureSetUp]
        public void Init()
        {
            MongoTestServer.SetMongodPtah(@"mongod\");
            this._mongoTestServer = MongoTestServer.Start(27017);
            ConfigManager.OverrideConnectionString(this._mongoTestServer.ConnectionString);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            this._mongoTestServer.Dispose();
        }
        
        #region Public Methods

        [Test]
        public void TestFindAnddOr()
        {
            //Llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            ConfigManager.Out = Console.Out;

            global::System.Collections.Generic.List<Country> Countries =
                MongoMapper.FindAsList<Country>(
                    Query.Or(MongoQuery.Eq((Country c) => c.Code, "ES"), Query.EQ("Code", "UK")));
            Assert.AreEqual(Countries.Count, 2);

            global::System.Collections.Generic.List<Person> Persons =
                MongoMapper.FindAsList<Person>(
                    Query.And(MongoQuery.Eq(((Person p) => p.Age), 25), Query.EQ("Country", "ES")));
            Assert.AreEqual(Persons.Count, 2);

            foreach (Person p in Persons)
            {
                Assert.AreEqual(p.Age, 25);
                Assert.AreEqual(p.Country, "ES");
            }
        }

        [Test]
        public void TestFindByPk()
        {
            //Llenamos datos
            Helper.DropAllCollections();

            ConfigManager.Out = Console.Out;

            var c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

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
                new Child { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });
            p.Childs.Add(
                new Child { ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" });

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

            p.Childs.Add(
                new Child { ID = 1, Age = 5, BirthDate = DateTime.Now.AddDays(7).AddYears(-5), Name = "Toni Sanchez" });

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

            p.Childs.Add(
                new Child { ID = 1, Age = 2, BirthDate = DateTime.Now.AddDays(2).AddYears(-2), Name = "Toni Serrano" });
            p.Save<Person>();

            p = new Person
                {
                    Name = "Jonh Smith",
                    Age = 21,
                    BirthDate = DateTime.Now.AddDays(21).AddYears(-21),
                    Married = false,
                    Country = "ES",
                    BankBalance = decimal.Parse("100,00")
                };

            p.Save<Person>();

            global::System.Collections.Generic.List<Person> plist = MongoMapper.AllAsList<Person>();

            var p2 = MongoMapper.FindByKey<Person>(plist[0].MongoMapper_Id);
            p2.Name = "FindBYKey Name";
            p2.Save<Person>();

            plist = MongoMapper.FindAsList<Person>("_id", p2.MongoMapper_Id);

            Assert.AreEqual(plist.Count, 1);
            Assert.AreEqual(plist[0].Name, "FindBYKey Name");
        }

        [Test]
        public void TestMongoCursor()
        {
            //llenamos datos
            (new InsertModifyDeleteTest()).TestInsert();

            //TODO: Falla el OutPut cuando ya hemos pedido el cursor ver como hacerlo
            //A MongoCursor object cannot be modified once it has been frozen
            ConfigManager.Out = null;

            global::System.Collections.Generic.List<Country> Countries = MongoMapper.FindAsCursor<Country>().ToList();
            Assert.AreEqual(Countries.Count, 3);

            Countries = MongoMapper.FindAsCursor<Country>().SetSkip(2).ToList();
            Assert.AreEqual(Countries.Count, 1);

            Countries = MongoMapper.FindAsCursor<Country>().SetLimit(1).ToList();
            Assert.AreEqual(Countries.Count, 1);

            Countries =
                MongoMapper.FindAsCursor<Country>(Query.EQ("Code", "ES")).SetFields(Fields.Include("Code")).ToList();
            Assert.AreEqual(Countries.Count, 1);
            Assert.AreEqual(Countries.First().Name, null);
        }

        #endregion
    }
}