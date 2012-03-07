using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class TestReplicaSet
    {
        public void Insert()
        {
            Helper.DropAllCollections();

            Parallel.For(0, 10, i =>
            {
                Country c = new Country { Code = i.ToString(), Name = String.Format("Nombre {0}", i) };
                c.Save<Country>();
            }
            );
        }

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
    }
}
