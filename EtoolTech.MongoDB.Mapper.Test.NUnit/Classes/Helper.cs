using System;
using System.IO;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Helper
    {
        #region Public Methods

        public static void DropAllCollections()
        {
            IMongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();


            var colNames = CollectionsManager.GetCollentionNames("XXX");

            foreach (string colName in colNames)
            {
                if (!colName.ToUpper().Contains("SYSTEM") && !colName.Contains("MongoMapperConfig"))
                {
                    Mapper.MongoMapperHelper.Db("XXX").DropCollectionAsync(colName);
                }
            }

            foreach (var collectionElement in config.CustomCollectionConfig)
            {
                var collection = (MongoMapperConfigurationElement)collectionElement;
                if (collection.Name != "TestConf1")
                {
                    foreach (string colName in CollectionsManager.GetCollentionNames(collection.Name))
                    {
                        if (!colName.ToUpper().Contains("SYSTEM") && !colName.Contains("MongoMapperConfig"))
                        {
                            Mapper.MongoMapperHelper.Db(collection.Name).DropCollectionAsync(colName);
                        }
                    }
                }
            }

        }

        #endregion
    }
}