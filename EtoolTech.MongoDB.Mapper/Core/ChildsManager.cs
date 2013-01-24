using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public class ChildsManager : IChildsManager
    {
        #region Constants and Fields

        private static readonly Dictionary<string, List<string>> FieldNamesBuffer =
            new Dictionary<string, List<string>>();

        private static IChildsManager _instance;
        private readonly Object _lockObject = new Object();

        #endregion

        #region Constructors and Destructors

        private ChildsManager()
        {
        }

        #endregion

        #region Public Properties

        public static IChildsManager Instance
        {
            get { return _instance ?? (_instance = new ChildsManager()); }
        }

        #endregion

        #region Public Methods

        public void GenerateChilsIds(string ObjName, IEnumerable<object> List)
        {
            foreach (object o in List)
            {
                var item = (IMongoMapperChildIdeable) o;
                item._id = ConfigManager.UseChildIncrementalId(ObjName)
                               ? MongoMapperIdGenerator.Instance.GenerateIncrementalId(o.GetType().Name)
                               : MongoMapperIdGenerator.Instance.GenerateId();
            }
        }

        public void ManageChilds(object Sender)
        {
            foreach (string pName in GeFieldNames(Sender))
            {
                object data = ReflectionUtility.GetPropertyValue(Sender, pName);
                var childList = (IEnumerable<object>) data;
                GenerateChilsIds(Sender.GetType().Name, childList);
            }
        }

        #endregion

        #region Methods

        private IEnumerable<string> GeFieldNames(object Sender)
        {
            string objName = Sender.GetType().Name;

            if (FieldNamesBuffer.ContainsKey(objName))
            {
                return FieldNamesBuffer[objName];
            }

            lock (_lockObject)
            {
                if (!FieldNamesBuffer.ContainsKey(objName))
                {
                    FieldNamesBuffer.Add(objName, new List<string>());

                    List<PropertyInfo> pList =
                        Sender.GetType().GetProperties().Where(
                            p => p.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault() != null).
                            ToList();

                    foreach (PropertyInfo p in pList)
                    {
                        FieldNamesBuffer[objName].Add(p.Name);
                    }
                }

                return FieldNamesBuffer[objName];
            }
        }

        #endregion
    }
}