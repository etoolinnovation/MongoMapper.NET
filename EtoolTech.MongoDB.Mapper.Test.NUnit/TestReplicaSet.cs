using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class TestReplicaSet
    {
        #region Public Methods

        public void Count()
        {
            Assert.AreEqual(10,(new CountryCollection()).Find().CountAsync().Result);
            Country c = (new CountryCollection()).Find().FirstAsync().Result;
            c.Delete();
        }

        public void Count2()
        {
            Assert.AreEqual(9, (new CountryCollection()).Find().CountAsync().Result);
        }

        public void Insert()
        {
            Helper.DropAllCollections();

            Parallel.For(
                0,
                10,
                i =>
                    {
                        var c = new Country {Code = i.ToString(), Name = String.Format("Nombre {0}", i)};
                        c.Save();
                    });
        }

        #endregion

        //private MongoTestServer _mongoTestServer;

        //[TestFixtureSetUp]
        //public void Init()
        //{
        //    MongoTestServer.SetMongodPtah(@"mongod\");
        //    this._mongoTestServer = MongoTestServer.Start(27017);
        //    ConfigManager.OverrideUrlString(this._mongoTestServer.ConnectionString);
        //}

        //[TestFixtureTearDown]
        //public void Dispose()
        //{
        //    this._mongoTestServer.Dispose();
        //}
    }
}