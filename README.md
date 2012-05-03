## MongoMapper.NET

A .NET Object Mapper for MongoDB over MongoDB C# Driver

* MongoDB distributions: http://www.mongodb.org/downloads

### Dependencies

* ServiceStack.Text: http://github.com/ServiceStack/ServiceStack.Text
* MongoDB Driver for CSharp: http://github.com/mongodb/mongo-csharp-driver
* NUnit: http://www.nunit.org, NUnit for Visual Studio: http://nunitforvs.codeplex.com

### Model

	[MongoKey(KeyFields = "Code")]
	public class Country: MongoMapper
	{        
		[MongoDownRelation(ObjectName = "Person", FieldName = "Country")]
		public string Code { get; set; }
		public string Name { get; set; }

		[MongoPropertyValidator(PropertyName="Code")]
		public void CodeIsUp(string CountryCode)
		{
			if (CountryCode != CountryCode.ToUpper())
				throw new Exception(String.Format("{0} must be {1}", CountryCode, CountryCode.ToUpper()));
		}
	}

	[MongoKey(KeyFields = "")]
	[MongoIndex(IndexFields = "ID,Country")]
	[MongoIndex(IndexFields =  "Name")]
	[MongoMapperIdIncrementable(IncremenalId = true, ChildsIncremenalId = true)]
	public class Person : MongoMapper
	{        
		public Person()
		{
			Childs = new List<Child>();
		}
				
		public string Name { get; set; }
		public int Age { get; set; }
		public DateTime BirthDate { get; set; }
		public bool Married { get; set; }
		public decimal BankBalance { get; set; }
		
		[MongoUpRelation(ObjectName = "Country", FieldName = "Code")]
		public string Country { get; set; }
			 
		[MongoChildCollection]
		public List<Child> Childs { get; set;}
	}
	

### Change Log

* 30/12/2011: http://bit.ly/uy80RE
* 05/01/2012: http://bit.ly/yfcsn4
* 02/02/2012: http://bit.ly/AfGfKC
* 14/02/2012: http://bit.ly/zvnk0F
* 07/03/2012: http://bit.ly/wMX6Ha
* 20/03/2012: http://bit.ly/GAIbez

### You can find examples in the Test Project 

* [Class Definition](https://github.com/emiliotorrens/MongoMapper.NET/tree/master/EtoolTech.MongoDB.Mapper.Test.NUnit/Classes) 
* [Find] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/FindTest.cs)
* [Write] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/InsertModifyDeleteTest.cs)
* [Incremental ID] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/IncrementalIdTest.cs)
* [Relations](https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/RelationsTest.cs) 
* [Events] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/EventsTest.cs)
* [Extension Methods] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/ExtensionTest.cs)
* [Original Version] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/OriginalObjectTest.cs)

