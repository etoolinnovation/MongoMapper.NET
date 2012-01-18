using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture()]
    public class EventsTest
    {
        [Test()]
        public void TestEvents()
        {
            Helper.DropAllDb();
            
            Country c = new Country { Code = "FR", Name = "España" };
            c.OnBeforeInsert += (s, e) => { ((Country)s).Name = "Francia"; };            
            c.Save<Country>();

            Country c3 = Country.FindByKey<Country>("FR");

            Assert.AreEqual(c3.Name, "Francia");

        }
    }
}
