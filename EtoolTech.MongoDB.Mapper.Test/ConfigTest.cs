using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestReadConfig()
        {
            MongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();

            Assert.AreEqual(config.Server.Host,"192.168.1.214");
            Assert.AreEqual(config.Server.Port, 27017);
            Assert.AreEqual(config.Database.User, "");
            Assert.AreEqual(config.Database.Password, "");
            Assert.AreEqual(config.Database.Name, "TestDotNET");
            Assert.AreEqual(config.Server.PoolSize, 5);

            
        }
    }
}
