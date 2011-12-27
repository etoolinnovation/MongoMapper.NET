using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class MongoQuery
    {
        private static readonly IFinder finder = new Finder();
        
        public static QueryComplete EQ<T>(Expression<Func<T, object>> fieldName, object value)
        {
            return finder.GetEqQuery(fieldName, value);
        }
    }
}
