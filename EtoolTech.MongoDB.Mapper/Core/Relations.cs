using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Relations : IRelations
    {
        private readonly Object _lockObjectUp = new Object();
        private readonly Object _lockObjectDown = new Object();

        private static readonly Dictionary<string, List<string>> BufferUpRelations =
            new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferDownRelations =
            new Dictionary<string, List<string>>();

        #region IRelations Members

        public List<string> GetUpRelations(Type t)
        {
            if (BufferUpRelations.ContainsKey(t.Name))
            {
                return BufferUpRelations[t.Name];
            }

            lock (_lockObjectUp)
            {
                if (!BufferUpRelations.ContainsKey(t.Name))
                {
                    var upRelations = new List<string>();
                    List<PropertyInfo> publicFieldInfos =
                        t.GetProperties().Where(
                            c => c.GetCustomAttributes(typeof (MongoUpRelation), false).FirstOrDefault() != null).ToList
                            (
                            );
                    foreach (PropertyInfo fieldInfo in publicFieldInfos)
                    {
                        var upRelationAtt =
                            (MongoUpRelation)
                            fieldInfo.GetCustomAttributes(typeof (MongoUpRelation), false).FirstOrDefault();

                        if (upRelationAtt != null)
                        {
                            upRelations.Add(
                                String.Format(
                                    "{0}|{1}|{2}|{3}|{4}",
                                    fieldInfo.Name,
                                    upRelationAtt.ObjectName,
                                    upRelationAtt.FieldName,
                                    upRelationAtt.ParentFieldName,
                                    upRelationAtt.ParentPropertyName));

                            if (!ConfigManager.Config.Context.Generated)
                            {
                                CollectionsManager.GetCollection(t.Name).EnsureIndex(fieldInfo.Name);
                            }
                        }
                    }
                    BufferUpRelations.Add(t.Name, upRelations);
                }

                return BufferUpRelations[t.Name];
            }
        }

        public List<string> GetDownRelations(Type t)
        {
            if (BufferDownRelations.ContainsKey(t.Name))
            {
                return BufferDownRelations[t.Name];
            }

            lock (_lockObjectDown)
            {
                if (!BufferDownRelations.ContainsKey(t.Name))
                {
                    var downRelations = new List<string>();
                    List<PropertyInfo> publicFieldInfos =
                        t.GetProperties().Where(
                            c => c.GetCustomAttributes(typeof (MongoDownRelation), false).FirstOrDefault() != null).
                            ToList();
                    foreach (PropertyInfo fieldInfo in publicFieldInfos)
                    {
                        object[] downRelationAtt = fieldInfo.GetCustomAttributes(typeof (MongoDownRelation), false);

                        foreach (object downRelation in downRelationAtt)
                        {
                            if (downRelation != null)
                            {
                                var relation = (MongoDownRelation) downRelation;
                                downRelations.Add(
                                    String.Format(
                                        "{0}|{1}|{2}", fieldInfo.Name, relation.ObjectName, relation.FieldName));
                                if (!ConfigManager.Config.Context.Generated)
                                {
                                    CollectionsManager.GetCollection(relation.ObjectName).EnsureIndex(
                                        relation.FieldName);
                                }
                            }
                        }
                    }

                    BufferDownRelations.Add(t.Name, downRelations);
                }

                return BufferDownRelations[t.Name];
            }
        }

        public List<T> GetRelation<T>(object sender, string Relation, Type ClassType, IFinder IFinder)
        {
            //c.GetRelation<Person>("Person,Country");
            string[] values = Relation.Split(',');

            string findString = String.Format("{0}|{1}", values[0], values[1]);

            string relationString =
                GetUpRelations(ClassType).FirstOrDefault(upRelation => upRelation.Contains(findString));
            if (String.IsNullOrEmpty(relationString))
            {
                relationString =
                    GetDownRelations(ClassType).FirstOrDefault(downRelation => downRelation.EndsWith(findString));
            }

            if (String.IsNullOrEmpty(relationString))
            {
                return new List<T>();
            }

            values = relationString.Split('|');
            string fieldName = values[0];
            string fkClassName = values[1];
            string fkFieldName = values[2];            
            object value = ReflectionUtility.GetPropertyValue(sender, fieldName);
            QueryComplete query = MongoQuery.Eq(fkFieldName, value);
            return
                CollectionsManager.GetCollection(String.Format("{0}_Collection", fkClassName)).FindAs<T>(query).ToList();
        }

        public void EnsureUpRelations(object sender, Type ClassType, IFinder IFinder)
        {
            //upRelations.Add(fieldInfo.Name + "|" + upRelationAtt.ObjectName + "|" + upRelationAtt.FieldName + 
            //"|" + upRelationAtt.ParentFieldName + "|" + upRelationAtt.ParentPropertyName);
            foreach (string UpRelation in GetUpRelations(ClassType))
            {
                string[] values = UpRelation.Split('|');
                string fieldName = values[0];
                string fkClassName = values[1];
                string fkFieldName = values[2];
                string fkParentfieldName = values[3];
                string fkParentPropertyName = values[4];

                QueryComplete ParentQuery = null;
                if (!String.IsNullOrEmpty(fkParentfieldName) && !String.IsNullOrEmpty(fkParentPropertyName))
                {                    
                    object Parentvalue = ReflectionUtility.GetPropertyValue(sender, fkParentPropertyName);
                    ParentQuery = MongoQuery.Eq(fkParentfieldName, Parentvalue);
                }
                
                object value = ReflectionUtility.GetPropertyValue(sender, fieldName); 
                QueryComplete query = MongoQuery.Eq(fkFieldName, value);

                if (ParentQuery != null)
                {
                    query = Query.And(ParentQuery, query);
                }

                MongoCollection fkCol = CollectionsManager.GetCollection(fkClassName + "_Collection");
                if (fkCol.Count(query) == 0)
                {
                    throw new ValidateUpRelationException(String.Format("{0}, Value:{1}", UpRelation, value));
                }
            }
        }

        public void EnsureDownRelations(object sender, Type ClassType, IFinder IFinder)
        {
            foreach (string Relation in GetDownRelations(ClassType))
            {
                string[] values = Relation.Split('|');
                string fieldName = values[0];
                string fkClassName = values[1];
                string fkFieldName = values[2];
                
                object value = ReflectionUtility.GetPropertyValue(sender, fieldName);
                QueryComplete query = MongoQuery.Eq(fkFieldName, value);

                MongoCollection fkCol = CollectionsManager.GetCollection(String.Format("{0}_Collection", fkClassName));
                if (fkCol.Count(query) != 0)
                {
                    throw new ValidateDownRelationException(String.Format("{0}, Value:{1}", Relation, value));
                }
            }
        }

        #endregion

        //TODO: Obtener las posibles Relaciones de las clases contenidas, solo UP
        public void GetChildsUpRelations(Type t)
        {
            List<PropertyInfo> publicFieldInfos =
                t.GetProperties().Where(
                    c => c.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault() != null).ToList();
            foreach (PropertyInfo fieldInfo in publicFieldInfos)
            {
                object mongoChildAtt =
                    fieldInfo.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault();
                if (mongoChildAtt != null)
                {
                    Type ChildType = ((MongoChildCollection) mongoChildAtt).ChildType;
                    List<string> Relations = GetUpRelations(ChildType);
                    int r = Relations.Count;
                }
            }
        }
    }
}