namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using EtoolTech.MongoDB.Mapper.Attributes;
    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Interfaces;

    public class ChildsManager : IChildsManager
    {
        #region Constants and Fields

        private static readonly Dictionary<string, List<string>> FieldNamesBuffer =
            new Dictionary<string, List<string>>();

        private readonly Object _lockObject = new Object();

        private static IChildsManager _instance;

        #endregion

        #region Constructors and Destructors

        private ChildsManager()
        {
        }

        #endregion

        #region Public Properties

        public static IChildsManager Instance
        {
            get
            {
                return _instance ?? (_instance = new ChildsManager());
            }
        }

        #endregion

        #region Public Methods

        public void GenerateChilsIds(string objName, IEnumerable<object> list)
        {
            foreach (object o in list)
            {
                var item = (IMongoMapperChildIdeable)o;
                item._id = ConfigManager.UseChildIncrementalId(objName)
                               ? MongoMapperIdGenerator.Instance.GenerateIncrementalId(o.GetType().Name)
                               : MongoMapperIdGenerator.Instance.GenerateId();
            }
        }

        public void ManageChilds(object sender)
        {
            foreach (string pName in this.GeFieldNames(sender))
            {
                object data = ReflectionUtility.GetPropertyValue(sender, pName);
                var childList = (IEnumerable<object>)data;
                this.GenerateChilsIds(sender.GetType().Name, childList);
            }
        }

        #endregion

        #region Methods

        private IEnumerable<string> GeFieldNames(object sender)
        {
            string objName = sender.GetType().Name;

            if (FieldNamesBuffer.ContainsKey(objName))
            {
                return FieldNamesBuffer[objName];
            }

            lock (this._lockObject)
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

        #endregion
    }
}