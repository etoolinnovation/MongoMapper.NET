using System;
using System.Diagnostics;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
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
            ReflectionUtility.BuildSchema(GetType().Assembly);
            ReflectionUtility.BuildSchema(GetType().Assembly,"Log");
        }

        [Test]
        public void TestGenerateSchema()
        {
            ReflectionUtility.BuildSchema(GetType().Assembly);
        }

        [Test]
        public void TestCheckRelations()
        {
            ReflectionUtility.CheckRelations(GetType().Assembly,"");
        }

        [Test]
        public void TestGetPropertyName()
        {
            string name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.String);
            Assert.AreEqual(name, "String");
            name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Int);
            Assert.AreEqual(name, "Int");
            name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Long);
            Assert.AreEqual(name, "Long");
            name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Decimal);
            Assert.AreEqual(name, "Decimal");
            name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Date);
            Assert.AreEqual(name, "Date");
            name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.Bool);
            Assert.AreEqual(name, "Bool");
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
    }
}