using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Exceptions;

namespace EtoolTech.MongoDB.Mapper
{
    public class PropertyValidator
    {
        #region Constants and Fields

        private static readonly Dictionary<string, MethodInfo> BufferPropertyValidatorMethods =
            new Dictionary<string, MethodInfo>();

        private static readonly Object LockObject = new Object();

        private static readonly List<string> ProcessedTypes = new List<string>();

        #endregion

        #region Public Methods

        public static void Validate(object Sender, Type T)
        {
            GetPropertyValidators(T);
            IEnumerable<KeyValuePair<string, MethodInfo>> validatorList = from b in BufferPropertyValidatorMethods
                                                                          where b.Key.StartsWith(T.Name + "|")
                                                                          select b;
            foreach (var v in validatorList)
            {
                ExecutePropertyValidator(Sender, v.Value, v.Key.Split('|')[1]);
            }
        }

        #endregion

        #region Methods

        private static void ExecutePropertyValidator(object Sender, MethodInfo Method, string PropertyName)
        {
            try
            {
                Method.Invoke(Sender, new[] {ReflectionUtility.GetPropertyValue(Sender, PropertyName)});
            }
            catch (Exception ex)
            {
                throw new ValidatePropertyException(
                    PropertyName, ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        private static void GetPropertyValidators(Type T)
        {
            if (ProcessedTypes.Contains(T.Name))
            {
                return;
            }

            lock (LockObject)
            {
                if (!ProcessedTypes.Contains(T.Name))
                {
                    ProcessedTypes.Add(T.Name);

                    List<MethodInfo> publicMethodInfos =
                        T.GetMethods().Where(
                            C => C.GetCustomAttributes(typeof (MongoPropertyValidator), false).FirstOrDefault() != null)
                            .
                            ToList();
                    foreach (MethodInfo methodInfo in publicMethodInfos)
                    {
                        var propValidatorAtt =
                            (MongoPropertyValidator)
                            methodInfo.GetCustomAttributes(typeof (MongoPropertyValidator), false).FirstOrDefault();
                        if (propValidatorAtt != null)
                        {
                            string className = T.Name;
                            string FieldName = propValidatorAtt.PropertyName;
                            string key = String.Format("{0}|{1}", className, FieldName);
                            if (!BufferPropertyValidatorMethods.ContainsKey(key))
                            {
                                BufferPropertyValidatorMethods.Add(key, methodInfo);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}