using System;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class Cursors
    {
        private void Cursors_OnGetCurrent(object sender, EventArgs e)
        {
            Country c = ((MongoCursor<Country>.OnEnumeratorGetCurrentEventArgs) e).Document;
            c.Code = "TESTCODE";
        }

        [Test]
        public void Test()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = string.Format("ES_{0}", i.ToString()), Name = "España"};
                c.Save();
                Assert.AreEqual(c.m_id, i + 1);
            }

            MongoCursor<Country> countries = MongoMapper.FindAsCursor<Country>();


            if (countries.GetType().GetInterface("IMongoCursorEvents") != null)
            {
                countries.OnEnumeratorGetCurrent += Cursors_OnGetCurrent;
            }


            foreach (Country c in countries)
            {
                Assert.AreEqual("TESTCODE", c.Code);
            }
        }
    }
}