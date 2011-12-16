using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    using EtoolTech.MongoDB.Mapper.Core;
    using EtoolTech.MongoDB.Mapper.Test.Classes;

    [TestClass]
    public class IncrementalIdTest
    {
        [TestMethod]
        public void TestIncId()
        {
            Helper.Db.Drop();

            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();
            Assert.AreEqual(c.MongoMapper_Id,1);


            c = new Country { Code = "US", Name = "EE UU" };
            c.Save<Country>();
            Assert.AreEqual(c.MongoMapper_Id, 2);


            c = new Country { Code = "UK", Name = "UK" };
            c.Save<Country>();
            Assert.AreEqual(c.MongoMapper_Id, 3);


            
        }
    }
}
