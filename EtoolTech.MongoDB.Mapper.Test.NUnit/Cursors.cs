using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Core.Interfaces;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class Cursors
    {
        [Test]
        public void Test()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country { Code = string.Format("ES_{0}", i.ToString()), Name = "España" };
                c.Save<Country>();
                Assert.AreEqual(c.MongoMapper_Id, i + 1);
            }

            MongoCursor<Country> countries = MongoMapper.FindAsCursor<Country>();
            

            if (countries.GetType().GetInterface("IMongoCursorEvents") != null)
            {
                countries.OnEnumeratorGetCurrent += Cursors_OnGetCurrent;
            }
           

            foreach (Country c in countries)
            {
                Assert.AreEqual("TESTCODE",c.Code);
            }

            

        }

        void Cursors_OnGetCurrent(object sender, System.EventArgs e)
        {
            Country c = ((MongoCursor<Country>.OnEnumeratorGetCurrentEventArgs)e).Document;
            c.Code = "TESTCODE";
        }
    }

  
}
