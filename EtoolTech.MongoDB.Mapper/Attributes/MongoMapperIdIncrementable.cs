using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoMapperIdIncrementable: Attribute
    {
        public bool IncremenalId { get; set; }
        public bool ChildsIncremenalId { get; set; }
    }
}
