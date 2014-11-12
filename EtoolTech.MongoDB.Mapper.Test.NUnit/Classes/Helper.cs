using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Helper
    {
        #region Public Methods

        public static void DropAllCollections()
        {
            IMongoMapperConfiguration config = MongoMapperConfiguration.GetConfig();

            foreach (string colName in Mapper.MongoMapperHelper.Db("XXX").GetCollectionNames())
            {
                if (!colName.ToUpper().Contains("SYSTEM"))
                {
                    Mapper.MongoMapperHelper.Db("XXX").GetCollection(colName).Drop();
                }
            }

            foreach (var collectionElement in config.CustomCollectionConfig)
            {
                var collection = (MongoMapperConfigurationElement)collectionElement;
                if (collection.Name != "TestConf1")
                {
                    foreach (string colName in Mapper.MongoMapperHelper.Db(collection.Name).GetCollectionNames())
                    {
                        if (!colName.ToUpper().Contains("SYSTEM"))
                        {
                            Mapper.MongoMapperHelper.Db(collection.Name).GetCollection(colName).Drop();
                        }
                    }
                }
            }
        }

        #endregion
    }
}