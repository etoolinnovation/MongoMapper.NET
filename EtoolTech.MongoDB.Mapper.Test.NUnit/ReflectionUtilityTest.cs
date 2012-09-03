namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Diagnostics;

    using EtoolTech.MongoDB.Mapper.Configuration;

    using global::NUnit.Framework;

    public class TestReflectionUtility
    {
        #region Public Properties

        public bool Bool { get; set; }

        public DateTime Date { get; set; }

        public decimal Decimal { get; set; }

        public int Int { get; set; }

        public long Long { get; set; }

        public string String { get; set; }

        #endregion
    }

    [TestFixture]
    public class ReflectionUtilityTest
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

        public static void TestObjectVsTypedNewGetPropertyValue()
        {
            var test = new TestReflectionUtility();
            test.String = "XXX";

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
            {
                ReflectionUtility.GetPropertyValue(test, "String");
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para obj: {0}", timer.Elapsed));

            timer = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                ReflectionUtility.GetPropertyValue<string>(test, "String");
            }

            timer.Stop();
            Console.WriteLine(string.Format("Elapsed para T: {0}", timer.Elapsed));
        }

        public void BuildSchema()
        {
            ReflectionUtility.BuildSchema(this.GetType().Assembly);
        }

        [Test]
        public void TestGetPropertyName()
        {
            string Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.String);
            Assert.AreEqual(Name, "String");
            Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Int);
            Assert.AreEqual(Name, "Int");
            Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Long);
            Assert.AreEqual(Name, "Long");
            Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Decimal);
            Assert.AreEqual(Name, "Decimal");
            Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Date);
            Assert.AreEqual(Name, "Date");
            Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Bool);
            Assert.AreEqual(Name, "Bool");
        }

        [Test]
        public void TestGetPropertyValue()
        {
            var test = new TestReflectionUtility();
            test.String = "XXX";
            test.Int = 999;
            test.Long = 999;
            test.Decimal = Decimal.Parse("999,5");
            test.Date = new DateTime(1972, 12, 5);
            test.Bool = true;

            Assert.AreEqual("XXX", ReflectionUtility.GetPropertyValue<string>(test, "String"));
            Assert.AreEqual(999, ReflectionUtility.GetPropertyValue<int>(test, "Int"));
            Assert.AreEqual(999, ReflectionUtility.GetPropertyValue<long>(test, "Long"));
            Assert.AreEqual(Decimal.Parse("999,5"), ReflectionUtility.GetPropertyValue<decimal>(test, "Decimal"));
            Assert.AreEqual(new DateTime(1972, 12, 5), ReflectionUtility.GetPropertyValue<DateTime>(test, "Date"));
            Assert.AreEqual(true, ReflectionUtility.GetPropertyValue<bool>(test, "Bool"));
        }

        #endregion
    }
}