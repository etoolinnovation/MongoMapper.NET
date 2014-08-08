using NUnit.Framework;
using MongoDB.Bson;
using EtoolTech.MongoDB.Mapper;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class AgregationTest
    {
		[Test]
		public void TestGroupSum()
		{
			var operations = new []{
				new BsonDocument {
					{
						"$group", new BsonDocument{ { "_id" , "$Country" } ,
							{"Total" , new BsonDocument{ {"$sum",1} } },
						}
					}
				}
			};

			var mongoresult = MongoMapper.Aggregate<Country>(operations);

			foreach (var document in mongoresult)
			{
				//TODO: document["_id"].ToString(), long.Parse(document["Total"].ToString()) });
			}


		}
    }
}