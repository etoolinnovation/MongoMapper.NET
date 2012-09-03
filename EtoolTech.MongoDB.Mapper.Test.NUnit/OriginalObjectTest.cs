namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Linq;

    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::NUnit.Framework;

    [TestFixture]
    public class OriginalObjectTest
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
        public void TestOriginalObject()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = MongoMapper.AllAsList<Person>().First();
            p.Save<Person>();
            p.Name = "hola 25";

            var p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Pepito Perez", p2.Name);
        }

        [Test]
        public void TestOriginalObjectWithOutSave()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = MongoMapper.AllAsList<Person>().First();
            p.Name = "hola 25";

            var p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Pepito Perez", p2.Name);
        }

		[Test]
        public void TestOriginalObjectCustom()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = MongoMapper.AllAsList<Person>().First();
            p.Name = "hola 25";

            var p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Pepito Perez", p2.Name);
			p.Save();

			p.Name = "Andres";
			p.SaveOriginal(true);
			p.Name = "Pepe";

			p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Andres", p2.Name);

        }


        [Test]
        public void TestOriginalValue()
        {
            Helper.DropAllCollections();

            var c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

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

            p.Name = "Juan Sin Miedo";

            object originalName = p.GetOriginalObject<Person>().Name;

            Assert.AreEqual(originalName.ToString(), "Pepito Perez");
        }

        #endregion
    }

}