using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{

    public class TestReflectionUtility
    {
        public string String { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public decimal Decimal { get; set; }
        public DateTime Date { get; set; }
        public bool Bool { get; set; }      
    }

    [TestClass]
    public class ReflectionUtilityTest
    {
        [TestMethod]
        public void TestGetPropertyName()
        {
            string Name = ReflectionUtility.GetPropertyName((TestReflectionUtility t) => t.String);
            Assert.AreEqual(Name,"String");
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
    }
}
