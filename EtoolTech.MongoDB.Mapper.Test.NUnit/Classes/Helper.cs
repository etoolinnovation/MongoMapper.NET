using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Helper
    {
        public static void DropAllDb()
        {
            MongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();
            Mapper.Helper.Db("XXX").Drop();

            foreach (CollectionElement collection in config.CollectionConfig)
            {
                if (collection.Name != "Fake") Mapper.Helper.Db(collection.Name).Drop();
            }
					
        }
    }
}
