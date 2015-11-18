using NUnit.Framework;
using MongoDB.Bson;
using EtoolTech.MongoDB.Mapper;
using System;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class AgregationTest
    {
		//[Test]
		//public void TestGroupSum()
		//{
		//	Helper.DropAllCollections();

		//	var c = new Country {Code = "NL", Name = "Holanda"};
		//	c.Save();

		//	c = new Country {Code = "ES", Name = "SPAIN"};
		//	c.Save();

		//	for (int i = 0; i < 10; i++) {
		//		var p = new Person
		//		{
		//			Name = i.ToString(),
		//			Age = i,
		//			BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
		//			Married = true,
		//			Country = "ES",
		//			BankBalance = decimal.Parse("3500,00")
		//		};
		//		p.Save();
		//	}

		//	for (int i = 0; i < 5; i++) {
		//		var p = new Person
		//		{
		//			Name = i.ToString(),
		//			Age = i,
		//			BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
		//			Married = true,
		//			Country = "NL",
		//			BankBalance = decimal.Parse("3500,00")
		//		};
		//		p.Save();
		//	}

		//	var operations = new []{
		//		new BsonDocument {
		//			{
		//				//$Country es $c
		//				"$group", new BsonDocument{ { "_id" , "$" + MongoMapperHelper.ConvertFieldName("Person","Country") } ,
		//					{"Total" , new BsonDocument{ {"$sum",1} } },
		//				}
		//			}
		//		}
		//	};

		//	var mongoresult = MongoMapper<Person>.Aggregate(operations);


		//	foreach (var document in mongoresult)
		//	{
		//		if (document ["_id"].ToString () == "ES")
		//			Assert.AreEqual (10, int.Parse (document ["Total"].ToString ()));
		//		else if (document ["_id"].ToString () == "NL")
		//			Assert.AreEqual (5, int.Parse (document ["Total"].ToString ()));
		//		else
		//			throw new InconclusiveException ("");
		//	}


		//}
    }
}