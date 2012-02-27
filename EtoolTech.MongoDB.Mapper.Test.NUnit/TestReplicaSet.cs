using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class TestReplicaSet
    {
        public void Test()
        {
            var c = MongoMapper.FindByKey<Country>("UK");
            c.Name = "TES REPLICA SET";
            c.Save<Country>();
        }
    }
}
