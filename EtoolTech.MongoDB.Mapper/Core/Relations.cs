using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class Relations : IRelations
    {        
        private static readonly Dictionary<string, List<string>> BufferUpRelations = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> BufferDownRelations = new Dictionary<string, List<string>>();        

        #region IRelations Members

        public List<string> GetUpRelations(Type t)
        {
            if (!BufferUpRelations.ContainsKey(t.Name))
            {
                var upRelations = new List<string>();
                List<PropertyInfo> publicFieldInfos = t.GetProperties().Where(c => c.GetCustomAttributes(typeof(MongoUpRelation), false).FirstOrDefault() != null).ToList();
                foreach (PropertyInfo fieldInfo in publicFieldInfos)
                {
                    var upRelationAtt =
                        (MongoUpRelation)
                        fieldInfo.GetCustomAttributes(typeof (MongoUpRelation), false).FirstOrDefault();

                    if (upRelationAtt != null)
                    {
                        upRelations.Add(String.Format("{0}|{1}|{2}|{3}|{4}", 
                                        fieldInfo.Name, upRelationAtt.ObjectName, upRelationAtt.FieldName, 
                                        upRelationAtt.ParentFieldName, upRelationAtt.ParentPropertyName));

                        if (!Context.ContextGenerated)
                            Helper.GetCollection(t.Name).EnsureIndex(fieldInfo.Name);
                    }
                }
                BufferUpRelations.Add(t.Name, upRelations);
            }

            return BufferUpRelations[t.Name];
        }

        public List<string> GetDownRelations(Type t)
        {
            if (!BufferDownRelations.ContainsKey(t.Name))
            {
                var downRelations = new List<string>();
                List<PropertyInfo> publicFieldInfos = t.GetProperties().Where(c => c.GetCustomAttributes(typeof(MongoDownRelation), false).FirstOrDefault() != null).ToList();
                foreach (PropertyInfo fieldInfo in publicFieldInfos)
                {
                    object[] downRelationAtt =
                        fieldInfo.GetCustomAttributes(typeof (MongoDownRelation), false);

                    foreach (object downRelation in downRelationAtt)
                    {
                        if (downRelation != null)
                        {
                            var relation = (MongoDownRelation) downRelation;
                            downRelations.Add(String.Format("{0}|{1}|{2}", 
                                              fieldInfo.Name, relation.ObjectName, relation.FieldName));
                            if (!Context.ContextGenerated)
                                Helper.GetCollection(relation.ObjectName).EnsureIndex(relation.FieldName);
                        }
                    }
                }

                BufferDownRelations.Add(t.Name, downRelations);
            }


            return BufferDownRelations[t.Name];
        }

        public List<T> GetRelation<T>(object sender, string Relation, Type ClassType, IFinder IFinder)
        {
            //c.GetRelation<Person>("Person,Country");
            string[] values = Relation.Split(',');

            string findString = String.Format("{0}|{1}", values[0], values[1]);

            string relationString =
                GetUpRelations(ClassType).Where(upRelation => upRelation.Contains(findString)).FirstOrDefault();
            if (String.IsNullOrEmpty(relationString))
            {
                relationString =
                    GetDownRelations(ClassType).Where(downRelation => downRelation.EndsWith(findString)).FirstOrDefault();
            }

            if (String.IsNullOrEmpty(relationString))
                return new List<T>();

            values = relationString.Split('|');
            string fieldName = values[0];
            string fkClassName = values[1];
            string fkFieldName = values[2];
            PropertyInfo propertyInfo = ClassType.GetProperty(fieldName);
            object value = propertyInfo.GetValue(sender, null);
            QueryComplete query = IFinder.GetEqQuery(propertyInfo.PropertyType, fkFieldName, value);
            return Helper.GetCollection(String.Format("{0}_Collection", fkClassName)).FindAs<T>(query).ToList();
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
                    PropertyInfo ParentPropertyInfo = ClassType.GetProperty(fkParentPropertyName);
                    object Parentvalue = ParentPropertyInfo.GetValue(sender, null);
                    ParentQuery = IFinder.GetEqQuery(ParentPropertyInfo.PropertyType, fkParentfieldName, Parentvalue);
                }

                PropertyInfo PropertyInfo = ClassType.GetProperty(fieldName);
                object value = PropertyInfo.GetValue(sender, null);
                var query = IFinder.GetEqQuery(PropertyInfo.PropertyType, fkFieldName, value);

                if (ParentQuery != null)
                {
                    query = Query.And(ParentQuery, query);
                }

                MongoCollection fkCol = Helper.GetCollection(fkClassName + "_Collection");
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

                PropertyInfo PropertyInfo = ClassType.GetProperty(fieldName);
                object value = PropertyInfo.GetValue(sender, null);
                var query = IFinder.GetEqQuery(PropertyInfo.PropertyType, fkFieldName, value);


                MongoCollection fkCol = Helper.GetCollection(String.Format("{0}_Collection", fkClassName));
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
            List<PropertyInfo> publicFieldInfos = t.GetProperties().Where(c => c.GetCustomAttributes(typeof(MongoChildCollection), false).FirstOrDefault() != null).ToList();
            foreach (PropertyInfo fieldInfo in publicFieldInfos)
            {
                var mongoChildAtt = fieldInfo.GetCustomAttributes(typeof(MongoChildCollection), false).FirstOrDefault();
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