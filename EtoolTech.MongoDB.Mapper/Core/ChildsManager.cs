using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Interfaces;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;

namespace EtoolTech.MongoDB.Mapper
{
	public class ChildsManager
	{
		private static ChildsManager _instance;
		
		public static ChildsManager Instance 
		{
			get 
			{
				if (_instance == null)
					_instance = new ChildsManager();
				return _instance;
			}
		}
		
		private ChildsManager () {}
		
		public void ManageChilds(object sender)
		{
											
			List<System.Reflection.PropertyInfo> pList = 
				sender.GetType().GetProperties().Where(p=>p.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault() != null).ToList();
						
			foreach(System.Reflection.PropertyInfo p in pList)
			{
				var data = ReflectionUtility.GetPropertyValue(sender, p.Name);
				var childList = (IEnumerable<object>) data;
				GenerateChilsIds(sender.GetType().Name, childList);				
			}
		}
				
		private void GenerateChilsIds(string objName, IEnumerable<object> list)
		{
			foreach (object o in list)
            {                
                IMongoMapperChildIdeable item = (IMongoMapperChildIdeable) o;
				if (ConfigManager.UseChildIncrementalId(objName))
				{
					item._id = MongoMapperIdGenerator.Instance.GenerateIncrementalId(o.GetType().Name);
				}
				else
				{
					item._id = MongoMapperIdGenerator.Instance.GenerateId();
				}
            }
		}

	}
}

