using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    

    [TestFixture()]
    public class IncrementalIdTest
    {
        [Test()]
        public void TestIncId()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                Country c = new Country { Code = "ES_"+i.ToString(), Name = "España" };
                c.Save<Country>();
                Assert.AreEqual(c.MongoMapper_Id, i+1);                
            }

            
        }
    }
}
