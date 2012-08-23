namespace EtoolTech.MongoDB.Mapper.Test.NUnit1
{
    using EtoolTech.MongoDB.Mapper.Test.NUnit;

    public class MyClass : IMyInterface
    {
        #region Public Properties

        public int Data { get; set; }

        public long _id { get; set; }

        #endregion
    }
}

namespace EtoolTech.MongoDB.Mapper.Test.NUnit2
{
    using EtoolTech.MongoDB.Mapper.Test.NUnit;

    public class MyClass : IMyInterface
    {
        #region Public Properties

        public int Data { get; set; }

        public long _id { get; set; }

        #endregion
    }
}

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    using System.Linq;

    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Test.NUnit1;

    using global::MongoDB.Driver;

    using global::NUnit.Framework;

    public interface IMyInterface
    {
        #region Public Properties

        int Data { get; set; }

        long _id { get; set; }

        #endregion
    }

    [TestFixture]
    public class AmbiguousDiscriminatorTest
    {
        private MongoTestServer _mongoTestServer;

        [TestFixtureSetUp]
        public void Init()
        {
            MongoTestServer.SetMongodPtah(@"mongod\");
            this._mongoTestServer = MongoTestServer.Start(27017);
            ConfigManager.OverrideConnectionString(this._mongoTestServer.ConnectionString);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {           
            this._mongoTestServer.Dispose();
        }
        #region Public Methods

        [Test]
        //Este test solo funcionara con el driver modificado
        public void Test()
        {
            MongoCollection<IMyInterface> col = Mapper.Helper.Db("XXX").GetCollection<IMyInterface>("MyClass");

            col.RemoveAll();

            var class1 = new MyClass { _id = 1, Data = 1 };
            var class2 = new NUnit2.MyClass { _id = 2, Data = 2 };

            col.Insert(class1);
            col.Insert(class2);

            global::System.Collections.Generic.List<IMyInterface> list = col.FindAll().ToList();
        }

        #endregion
    }
}