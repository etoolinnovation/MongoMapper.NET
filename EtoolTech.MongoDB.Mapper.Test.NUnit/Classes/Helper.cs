using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Helper
    {
        public static void DropAllCollections()
        {
            MongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();
            
			foreach(string colName in Mapper.Helper.Db("XXX").GetCollectionNames())
			{
				if (!colName.ToUpper().Contains("SYSTEM"))
				{
					Mapper.Helper.Db("XXX").GetCollection(colName).Drop();
				}
			}

            foreach (CollectionElement collection in config.CollectionConfig)
            {
                if (collection.Name != "TestConf1") 
				{
					foreach(string colName in Mapper.Helper.Db(collection.Name).GetCollectionNames())
					{
						if (!colName.ToUpper().Contains("SYSTEM"))
						{
							Mapper.Helper.Db(collection.Name).GetCollection(colName).Drop();
						}
					}
				}
            }
					
        }
    }
}
