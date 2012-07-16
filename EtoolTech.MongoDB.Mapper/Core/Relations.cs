namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using EtoolTech.MongoDB.Mapper.Attributes;
    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Exceptions;
    using EtoolTech.MongoDB.Mapper.Interfaces;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public class Relations : IRelations
    {
        #region Constants and Fields

        private static readonly Dictionary<string, List<string>> BufferDownRelations =
            new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, List<string>> BufferUpRelations =
            new Dictionary<string, List<string>>();

        private readonly Object _lockObjectDown = new Object();

        private readonly Object _lockObjectUp = new Object();

        private static IRelations _relations;

        #endregion

        #region Constructors and Destructors

        private Relations()
        {
        }

        #endregion

        #region Public Properties

        public static IRelations Instance
        {
            get
            {
                return _relations ?? (_relations = new Relations());
            }
        }

        #endregion

        #region Public Methods

        public void EnsureDownRelations(object sender, Type classType, IFinder finder)
        {
            foreach (string relation in this.GetDownRelations(classType))
            {
                string[] values = relation.Split('|');
                string fieldName = values[0];
                string fkClassName = values[1];
                string fkFieldName = values[2];

                object value = ReflectionUtility.GetPropertyValue(sender, fieldName);
                IMongoQuery query = MongoQuery.Eq(fkFieldName, value);

                MongoCollection fkCol =
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(fkClassName));
                if (fkCol.Count(query) != 0)
                {
                    throw new ValidateDownRelationException(String.Format("{0}, Value:{1}", relation, value));
                }
            }
        }

        public void EnsureUpRelations(object sender, Type classType, IFinder finder)
        {
            //upRelations.Add(fieldInfo.Name + "|" + upRelationAtt.ObjectName + "|" + upRelationAtt.FieldName + 
            //"|" + upRelationAtt.ParentFieldName + "|" + upRelationAtt.ParentPropertyName);
            foreach (string upRelation in this.GetUpRelations(classType))
            {
                string[] values = upRelation.Split('|');
                string fieldName = values[0];
                string fkClassName = values[1];
                string fkFieldName = values[2];
                string fkParentfieldName = values[3];
                string fkParentPropertyName = values[4];

                IMongoQuery parentQuery = null;
                if (!String.IsNullOrEmpty(fkParentfieldName) && !String.IsNullOrEmpty(fkParentPropertyName))
                {
                    object parentvalue = ReflectionUtility.GetPropertyValue(sender, fkParentPropertyName);
                    parentQuery = MongoQuery.Eq(fkParentfieldName, parentvalue);
                }

                object value = ReflectionUtility.GetPropertyValue(sender, fieldName);
                IMongoQuery query = MongoQuery.Eq(fkFieldName, value);

                if (parentQuery != null)
                {
                    query = Query.And(parentQuery, query);
                }

                MongoCollection fkCol =
                    CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(fkClassName));
                if (fkCol.Count(query) == 0)
                {
                    throw new ValidateUpRelationException(String.Format("{0}, Value:{1}", upRelation, value));
                }
            }
        }

        public List<string> GetDownRelations(Type t)
        {
            if (BufferDownRelations.ContainsKey(t.Name))
            {
                return BufferDownRelations[t.Name];
            }

            lock (this._lockObjectDown)
            {
                if (!BufferDownRelations.ContainsKey(t.Name))
                {
                    var downRelations = new List<string>();
                    List<PropertyInfo> publicFieldInfos =
                        t.GetProperties().Where(
                            c => c.GetCustomAttributes(typeof(MongoDownRelation), false).FirstOrDefault() != null).
                            ToList();
                    foreach (PropertyInfo fieldInfo in publicFieldInfos)
                    {
                        object[] downRelationAtt = fieldInfo.GetCustomAttributes(typeof(MongoDownRelation), false);

                        foreach (object downRelation in downRelationAtt)
                        {
                            if (downRelation != null)
                            {
                                var relation = (MongoDownRelation)downRelation;
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

        public List<T> GetRelation<T>(object sender, string relation, Type classType, IFinder finder)
        {
            //c.GetRelation<Person>("Person,Country");
            string[] values = relation.Split(',');

            string findString = String.Format("{0}|{1}", values[0], values[1]);

            string relationString =
                this.GetUpRelations(classType).FirstOrDefault(upRelation => upRelation.Contains(findString));
            if (String.IsNullOrEmpty(relationString))
            {
                relationString =
                    this.GetDownRelations(classType).FirstOrDefault(downRelation => downRelation.EndsWith(findString));
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
            IMongoQuery query = MongoQuery.Eq(fkFieldName, value);
            return
                CollectionsManager.GetCollection(String.Format("{0}_Collection", fkClassName)).FindAs<T>(query).ToList();
        }

        public List<string> GetUpRelations(Type t)
        {
            if (BufferUpRelations.ContainsKey(t.Name))
            {
                return BufferUpRelations[t.Name];
            }

            lock (this._lockObjectUp)
            {
                if (!BufferUpRelations.ContainsKey(t.Name))
                {
                    var upRelations = new List<string>();
                    List<PropertyInfo> publicFieldInfos =
                        t.GetProperties().Where(
                            c => c.GetCustomAttributes(typeof(MongoUpRelation), false).FirstOrDefault() != null).ToList(
                                );
                    foreach (PropertyInfo fieldInfo in publicFieldInfos)
                    {
                        var upRelationAtt =
                            (MongoUpRelation)
                            fieldInfo.GetCustomAttributes(typeof(MongoUpRelation), false).FirstOrDefault();

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

        #endregion

        //TODO: Obtener las posibles Relaciones de las clases contenidas, solo UP
        //public void GetChildsUpRelations(Type t)
        //{
        //    List<PropertyInfo> publicFieldInfos =
        //        t.GetProperties().Where(
        //            c => c.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault() != null).ToList();
        //    foreach (PropertyInfo fieldInfo in publicFieldInfos)
        //    {
        //        object mongoChildAtt =
        //            fieldInfo.GetCustomAttributes(typeof (MongoChildCollection), false).FirstOrDefault();
        //        if (mongoChildAtt != null)
        //        {
        //            Type ChildType = ((MongoChildCollection) mongoChildAtt).ChildType;
        //            List<string> Relations = GetUpRelations(ChildType);
        //            int r = Relations.Count;
        //        }
        //    }
        //}
    }
}