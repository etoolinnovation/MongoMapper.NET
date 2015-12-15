using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public static class ReflectionUtility
    {
        #region Public Methods

        public static void BuildSchema(Assembly Assembly)
        {
            BuildSchema(Assembly, String.Empty);
        }


        public static void BuildSchema(Assembly Assembly, string ClassName)
        {
            List<Type> types = String.IsNullOrEmpty(ClassName) ? Assembly.GetTypes().Where(T => T.BaseType != null && T.BaseType.Name == "MongoMapper`1").ToList() : 
                Assembly.GetTypes().Where(T => T.BaseType != null && T.BaseType.Name == "MongoMapper`1" && T.Name == ClassName).ToList();

            foreach (Type type in types)
            {
                MongoMapperHelper.RebuildClass(type, true);
            }
        }


        public static void CheckRelations(Assembly Assembly, string ClassName)
        {
            List<Type> types = String.IsNullOrEmpty(ClassName) ? Assembly.GetTypes().Where(T => T.BaseType != null && T.BaseType.Name == "MongoMapper`1").ToList() :
                Assembly.GetTypes().Where(T => T.BaseType != null && T.BaseType.Name == "MongoMapper`1" && T.Name == ClassName).ToList();

            foreach (Type type in types)
            {

                Console.WriteLine("CHECKING OBJECT {0}", type.Name);

                object[] relationAttList = type.GetCustomAttributes(typeof(MongoRelation), false);

                var relations = relationAttList.Where(RelationAtt => RelationAtt != null).Cast<MongoRelation>().ToList();

                foreach (var relation in relations)
                {

                    Console.WriteLine("     CHECKING RELATION {0}", relation.Name);

                    //Check Local Fields Exists
                    var localPropertiesNames = type.GetProperties().Select(P => P.Name).ToList();
                    foreach (var relationFieldName in relation.CurrentFieldNames)
                    {
                        if (!localPropertiesNames.Contains(relationFieldName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("     WARNING: field {0} does not exists in {1}", relationFieldName, type.Name);
                            Console.ResetColor();
                        }
                    }



                    //Check if destination Object exist
                    var destinationObj =
                        Assembly.GetTypes().Where(T =>
                                    T.BaseType != null && T.BaseType.Name == "MongoMapper`1" &&
                                    T.Name == relation.RelationObjectName).ToList();

                    //Check if destinatin Fields Exists
                    if (destinationObj.Any())
                    {
                        var destinationType = destinationObj.First();

                        var destinationPropertiesNames = destinationType.GetProperties().Select(P => P.Name).ToList();
                        foreach (var relationFieldName in relation.RelationFieldNames)
                        {
                            if (!destinationPropertiesNames.Contains(relationFieldName))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("     WARNING: field {0} does not exists in {1}", relationFieldName, destinationType.Name);
                                Console.ResetColor();
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("     WARNING: {0} does not exists in {1}", relation.RelationObjectName, Assembly.FullName);
                        Console.ResetColor();
                    }
                }
            }
        }



        // object[] relationAttList = T.GetCustomAttributes(typeof(MongoRelation), false);


        public static void CopyObject<T>(T Source, object Destination)
        {
            Type t = Source.GetType();

            foreach (PropertyInfo property in t.GetProperties().Where(P=>P.GetSetMethod() != null))
            {
                property.SetValue(Destination, property.GetValue(Source, null), null);
            }
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> Exp)
        {
            var memberExpression = Exp.Body as UnaryExpression;
            if (memberExpression != null)
            {
                var memberexpresion2 = memberExpression.Operand as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }
            else
            {
                var memberexpresion2 = Exp.Body as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }

            return string.Empty;
        }

        public static string GetPropertyName<T>(Expression<Func<T, string>> Exp)
        {
            var memberExpression = Exp.Body as UnaryExpression;
            if (memberExpression != null)
            {
                var memberexpresion2 = memberExpression.Operand as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }
            else
            {
                var memberexpresion2 = Exp.Body as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }

            return string.Empty;
        }

        public static object GetPropertyValue(object Obj, string PropertyName)
        {
            if ((PropertyName == "m_id") && (Obj is IMongoMapperIdeable))
            {
                return ((IMongoMapperIdeable) Obj).m_id;
            }

            Type t = Obj.GetType();
            PropertyInfo property = t.GetProperty(PropertyName);
            return property.GetValue(Obj, null);
        }

        public static T GetPropertyValue<T>(object Obj, string PropertyName)
        {
            return (T) GetPropertyValue(Obj, PropertyName);
        }

        #endregion
    }
}