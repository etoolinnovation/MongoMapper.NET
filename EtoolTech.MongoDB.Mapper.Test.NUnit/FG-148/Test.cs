using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtoolTech.Orca.BedBank.BlackBox.Core.Data.Mongo;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    public class Test
    {
        public void Filter()
        {

            var Fields = new string[] { "Description" };
            var OrderBy = new string[] { "lmd","CurrencyCode" };

            string Name = "%";
            int Skip = 25;
            int Limit = 25;

            var col = MongoMapperCollection<CurrencyType>.Instance;
            Fields = MongoMapperHelper.ConvertFieldName("CurrencyType", Fields.ToList()).ToArray();

            OrderBy = MongoMapperHelper.ConvertFieldName("CurrencyType", OrderBy.ToList()).ToArray();
            var sortList = new List<SortDefinition<CurrencyType>>();

            if (!OrderBy.Any() || string.IsNullOrEmpty(OrderBy.First())) sortList.Add(col.Sort.Ascending("$natural"));

            sortList.AddRange(OrderBy.Where(S => !string.IsNullOrEmpty(S)).Select(Field => col.Sort.Ascending(Field)));

            var order = col.Sort.Combine(sortList);

            col.AddIncludeFields(Fields);

            col.Find(col.Filter.And(MongoQuery<CurrencyType>.Eq(D => D.Name, Name))).Limit(Limit).Skip(Skip).Sort(order);
            

            Console.WriteLine(col.Count);
            foreach (var currencyType in col)
            {
                Console.WriteLine(currencyType.CurrencyCode);
                Console.WriteLine(currencyType.Description);

            }
            
            
        }
    }
}
