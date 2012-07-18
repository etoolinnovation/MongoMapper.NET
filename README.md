## MongoMapper.NET

A .NET Object Mapper for MongoDB over MongoDB C# Driver



### Dependencies

* [MongoDB distributions] (http://www.mongodb.org/downloads)
* [MongoDB Driver for CSharp] (http://github.com/mongodb/mongo-csharp-driver)
* [NUnit] (http://www.nunit.org), [NUnit for Visual Studio] (http://nunitforvs.codeplex.com)

### Defining the Model

	[Serializable]
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

	[Serializable]
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
	
### Work with the Model

		Country c = new Country {Code = "es", Name = "España"};
		try
		{
			c.Save();
		}
		catch(Exception ex)
		{
			Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidatePropertyException)); 
			c.Code = "ES";
			c.Save();
		}
		
		c = new Country { Code = "UK", Name = "Reino Unido" };
		c.Save();
		
		c = new Country { Code = "UK", Name = "Reino Unido" };
		try
		{
			c.Save();
		}
		catch (Exception ex)
		{
			Assert.AreEqual(ex.GetBaseException().GetType(),typeof(DuplicateKeyException));	
		}
		
		Country c2 = new Country { Code = "US", Name = "Francia" };
        c2.OnBeforeInsert += (s, e) => { ((Country)s).Name = "Estados Unidos"; };            
        c2.Save();

        Country c3 = new Country();
		c3.FillByKey("US");
        Assert.AreEqual(c3.Name, "Estados Unidos");
		
		List<Country> countries = new List<Country>();
		countries.MongoFind();
		Assert.AreEqual(countries.Count, 3);

		countries.MongoFind(Query.Or(MongoQuery.Eq((Country co) => co.Code, "ES"), MongoQuery.Eq((Country co) => co.Code, "UK")));
		Assert.AreEqual(countries.Count, 2);
		
		Person p = new Person
		{
			Name = "Pepito Perez",
			Age = 35,
			BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
			Married = true,
			Country = "XXXXX",
			BankBalance = decimal.Parse("3500,00")
		};

		p.Childs.Add(new Child() { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });		

		try
        {
            p.Save();
        }
        catch (Exception ex)
        {
			Assert.AreEqual(ex.GetBaseException().GetType(), typeof(ValidateUpRelationException));
            p.Country = "ES";
            p.Save();
        }	

		p.ServerUpdate(Update.PushWrapped("Childs", new Child() { ID = 2, Age = 2, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" }));		
		
		List<Person> persons = new List<Person>();
		persons.MongoFind("Childs.Age",2);
		Assert.AreEqual(persons.Count, 1);


### You can find examples in the Test Project 

* [Class Definition](https://github.com/emiliotorrens/MongoMapper.NET/tree/master/EtoolTech.MongoDB.Mapper.Test.NUnit/Classes) 
* [Find] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/FindTest.cs)
* [Write] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/InsertModifyDeleteTest.cs)
* [Incremental ID] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/IncrementalIdTest.cs)
* [Relations](https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/RelationsTest.cs) 
* [Events] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/EventsTest.cs)
* [Extension Methods] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/ExtensionTest.cs)
* [Original Version] (https://github.com/emiliotorrens/MongoMapper.NET/blob/master/EtoolTech.MongoDB.Mapper.Test.NUnit/OriginalObjectTest.cs)
 
### Change Log

* [30/12/2011] (http://bit.ly/uy80RE)
* [05/01/2012] (http://bit.ly/yfcsn4)
* [02/02/2012] (http://bit.ly/AfGfKC)
* [14/02/2012] (http://bit.ly/zvnk0F)
* [07/03/2012] (http://bit.ly/wMX6Ha)
* [20/03/2012] (http://bit.ly/GAIbez)
* [18/07/2012] (http://bit.ly/M9gqOp) 

