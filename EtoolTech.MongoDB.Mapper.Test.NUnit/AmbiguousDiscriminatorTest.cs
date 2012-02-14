using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EtoolTech.MongoDB.Mapper.Test.NUnit;
using EtoolTech.MongoDB.Mapper.Test.NUnit1;
using MongoDB.Driver;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit1
{
    public class MyClass: IMyInterface
    {
        public long _id { get; set; }
        public int Data { get; set; }
    }
}

namespace EtoolTech.MongoDB.Mapper.Test.NUnit2
{
    public class MyClass : IMyInterface
    {
        public long _id { get; set; }
        public int Data { get; set; }
    }
}


namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public interface IMyInterface
    {
        long _id { get; set; }
        int Data { get; set; }
    }
    
    
    [TestFixture]
    public class AmbiguousDiscriminatorTest
    {
        [Test]
        //Este test solo funcionara con el driver modificado
        public void Test()
        {
            MongoCollection<IMyInterface> col = Mapper.Helper.Db("XXX").GetCollection<IMyInterface>("MyClass");

            col.RemoveAll();

            NUnit1.MyClass class1 = new NUnit1.MyClass() { _id = 1, Data = 1 };
            NUnit2.MyClass class2 = new NUnit2.MyClass() {_id = 2, Data = 2};

            col.Insert(class1);
            col.Insert(class2);

            List<IMyInterface> list = col.FindAll().ToList();
        }
    }
}
