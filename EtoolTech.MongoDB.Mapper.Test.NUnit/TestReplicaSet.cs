namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::NUnit.Framework;

    public class TestReplicaSet
    {
        #region Public Methods

        public void Count()
        {
            Assert.AreEqual(10, MongoMapper.FindAsCursor<Country>().Size());
            Country c = MongoMapper.FindAsCursor<Country>().First();
            c.Delete<Country>();
        }

        public void Count2()
        {
            Assert.AreEqual(9, MongoMapper.FindAsCursor<Country>().Size());
        }

        public void Insert()
        {
            Helper.DropAllCollections();

            Parallel.For(
                0,
                10,
                i =>
                    {
                        var c = new Country { Code = i.ToString(), Name = String.Format("Nombre {0}", i) };
                        c.Save<Country>();
                    });
        }

        #endregion
    }
}