using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    

    [TestFixture()]
    public class OriginalObjectTest
    {
        [Test()]
        public void TestOriginalObject()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = Person.AllAsList<Person>().First();            
            p.Save<Person>();
            p.Name = "hola 25";            
            

            Person p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Pepito Perez", p2.Name);
        }


        [Test()]
        public void TestOriginalObjectWithOutSave()
        {
            (new InsertModifyDeleteTest()).TestInsert();
            Person p = Person.AllAsList<Person>().First();
            //TODO: ver donde colocarlo para que lo haga solo tras la deserializacion
            p.SaveOriginal();
            p.Name = "hola 25";


            Person p2 = p.GetOriginalObject<Person>();
            Assert.AreEqual("Pepito Perez", p2.Name);
        }


        [Test()]
        public void TestOriginalValue()
        {

            Helper.DropAllCollections();
            
            Country c = new Country { Code = "ES", Name = "España" };
            c.Save<Country>();

            Person p = new Person
            {                
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

            object originalName = p.GetOriginalObject<Person>().Name;

            Assert.AreEqual(originalName.ToString(), "Pepito Perez");


        }
    }
}
