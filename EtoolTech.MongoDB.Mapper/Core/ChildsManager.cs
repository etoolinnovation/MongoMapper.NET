using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Interfaces;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper
{
    public class ChildsManager : IChildsManager
    {
		private static IChildsManager _instance;
        private static readonly Dictionary<string,List<string>> FieldNamesBuffer = new Dictionary<string, List<string>>();
        private readonly Object _lockObject = new Object();
		
		public static IChildsManager Instance 
		{
			get
			{
			    return _instance ?? (_instance = new ChildsManager());
			}
		}
		
		private ChildsManager () {}
		
        private IEnumerable<string> GeFieldNames(object sender)
        {
            string objName = sender.GetType().Name;

            if (FieldNamesBuffer.ContainsKey(objName)) return FieldNamesBuffer[objName];

            lock (_lockObject)
            {

                if (!FieldNamesBuffer.ContainsKey(objName))
                {
                    FieldNamesBuffer.Add(objName, new List<string>());

                    List<PropertyInfo> pList =
                        sender.GetType().GetProperties().Where(
                            p => p.GetCustomAttributes(typeof(MongoChildCollection), false).FirstOrDefault() != null).
                            ToList();

                    foreach (PropertyInfo p in pList)
                    {
                        FieldNamesBuffer[objName].Add(p.Name);
                    }

                }

                return FieldNamesBuffer[objName];
            }
        }

		public void ManageChilds(object sender)
		{														
            foreach (string pName in this.GeFieldNames(sender))
			{
				var data = ReflectionUtility.GetPropertyValue(sender, pName);
				var childList = (IEnumerable<object>) data;
				GenerateChilsIds(sender.GetType().Name, childList);				
			}
		}

        public void GenerateChilsIds(string objName, IEnumerable<object> list)
		{
			foreach (object o in list)
			{
			    var item = (IMongoMapperChildIdeable) o;
			    item._id = ConfigManager.UseChildIncrementalId(objName) ? MongoMapperIdGenerator.Instance.GenerateIncrementalId(o.GetType().Name) : MongoMapperIdGenerator.Instance.GenerateId();
			}
		}

	}
}

