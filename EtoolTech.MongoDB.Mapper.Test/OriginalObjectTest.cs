using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    using EtoolTech.MongoDB.Mapper.Test.Classes;

    [TestClass]
    public class OriginalObjectTest
    {
        [TestMethod]
        public void TestOriginalObject()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = Person.AllAsList<Person>().First();
            p.Name = "hola 25";
            p.Save<Person>();

            string Name = p.GetOriginalValue<Person>("Name").ToString();
            Assert.AreEqual(Name,"Pepito Perez");

            Person p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual(Name, p2.Name);
        }

        [TestMethod]
        public void TestOriginalValue()
        {

            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            Person p = new Person
            {
                Id = 1,
                Name = "Pepito Perez",
                Age = 35,
                BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                Married = true,
                Country = "ES",
                BankBalance = decimal.Parse("3500,00")
            };

            p.Childs.Add(new Child() { ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez" });
            p.Childs.Add(new Child() { ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez" });
            p.Save<Person>();
       
            p.Name = "Juan Sin Miedo";

            object OriginalName = p.GetOriginalValue<Person>("Name");

            Assert.AreEqual(OriginalName.ToString(), "Pepito Perez");


        }
    }
}
