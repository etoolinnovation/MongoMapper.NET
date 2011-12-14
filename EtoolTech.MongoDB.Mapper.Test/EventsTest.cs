using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Core;
using EtoolTech.MongoDB.Mapper.Test.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class EventsTest
    {
        [TestMethod]
        public void TestEvents()
        {
            Helper.Db.Drop();
            
            Country c = new Country { Code = "FR", Name = "España" };
            c.OnBeforeInsert += (s, e) => { ((Country)s).Name = "Francia"; };
            c.OnBeforeModify += (s, e) => { ((Country)s).Name = "Francia"; };
            c.Save<Country>();

            Country c3 = Country.FindByKey<Country>("FR");

            Assert.AreEqual(c3.Name, "Francia");

        }
    }
}
