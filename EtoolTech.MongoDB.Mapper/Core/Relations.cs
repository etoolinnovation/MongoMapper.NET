using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EtoolTech.MongoDB.Mapper.Attributes;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class Relations<T> : IRelations<T>
    {
        #region Constants and Fields

        private static readonly Dictionary<string, List<MongoRelation>> BufferDownRelations = new Dictionary<string, List<MongoRelation>>();

        private static readonly Dictionary<string, List<MongoRelation>> BufferUpRelations = new Dictionary<string, List<MongoRelation>>();

        private static IRelations<T> _relations;

        private readonly Object _lockObjectDown = new Object();

        private readonly Object _lockObjectUp = new Object();

        #endregion

        #region Constructors and Destructors

        private Relations()
        {
        }

        #endregion

        #region Public Properties

        public static IRelations<T> Instance
        {
            get { return _relations ?? (_relations = new Relations<T>()); }
        }

        #endregion

        #region Public Methods

        public void EnsureDownRelations(object Sender, Type ClassType, IFinder Finder)
        {
            foreach (MongoRelation relation in GetDownRelations(ClassType))
            {
                var filters = relation.CurrentFieldNames.Select(
                    T => ReflectionUtility.GetPropertyValue(Sender, T)).Select((Value, I) => 
                    MongoQuery<BsonDocument>.Eq(relation.RelationObjectName, relation.RelationFieldNames[I], Value)).ToList();

                var relationCollection = CollectionsManager.GetCollection<BsonDocument>(relation.RelationObjectName);
                if (relationCollection.CountAsync(Builders<BsonDocument>.Filter.And(filters)).Result != 0)
                {
                    throw new ValidateDownRelationException(filters.ToJson());
                }
            }
        }

        public void EnsureUpRelations(object Sender, Type ClassType, IFinder Finder)
        {
            foreach (MongoRelation relation in GetDownRelations(ClassType))
            {
                var filters = relation.CurrentFieldNames.Select(
                    T => ReflectionUtility.GetPropertyValue(Sender, T)).Select((Value, I) =>
                    MongoQuery<BsonDocument>.Eq(relation.RelationObjectName, relation.RelationFieldNames[I], Value)).ToList();

                var relationCollection = CollectionsManager.GetCollection<BsonDocument>(relation.RelationObjectName);
                if (relationCollection.CountAsync(Builders<BsonDocument>.Filter.And(filters)).Result != 0)
                {
                    throw new ValidateDownRelationException(filters.ToJson());
                }
            }
        }

        public List<MongoRelation> GetDownRelations(Type T)
        {
            if (BufferDownRelations.ContainsKey(T.Name))
            {
                return BufferDownRelations[T.Name];
            }

            lock (_lockObjectDown)
            {
                if (!BufferDownRelations.ContainsKey(T.Name))
                {
                    object[] relationAttList = T.GetCustomAttributes(typeof(MongoRelation), false);

                    var downRelations = relationAttList.Where(relationAtt => relationAtt != null && !((MongoRelation) relationAtt).UpRelation).Cast<MongoRelation>().ToList();

                    BufferDownRelations.Add(T.Name, downRelations);
                }

                return BufferDownRelations[T.Name];
            }
        }

        public List<T> GetRelation<T>(object Sender, string Relation, Type ClassType, IFinder Finder)
        {
            //TODO
            ////c.GetRelation<Person>("Person,Country");
            //string[] values = Relation.Split(',');

            //string findString = String.Format("{0}|{1}", values[0], values[1]);

            //string relationString =
            //    GetUpRelations(ClassType).FirstOrDefault(UpRelation => UpRelation.Contains(findString));
            //if (String.IsNullOrEmpty(relationString))
            //{
            //    relationString =
            //        GetDownRelations(ClassType).FirstOrDefault(DownRelation => DownRelation.EndsWith(findString));
            //}

            //if (String.IsNullOrEmpty(relationString))
            //{
            //    return new List<T>();
            //}

            //values = relationString.Split('|');
            //string fieldName = values[0];
            //string fkClassName = values[1];
            //string fkFieldName = values[2];

            //object value = ReflectionUtility.GetPropertyValue(Sender, fieldName);
            //var query = MongoQuery<T>.Eq(typeof(T).Name, fkFieldName, value);

            //return CollectionsManager.GetCollection<T>(fkClassName).Find(query).ToListAsync().Result;
            return default(List<T>);
        }

        public List<MongoRelation> GetUpRelations(Type T)
        {

            if (BufferDownRelations.ContainsKey(T.Name))
            {
                return BufferDownRelations[T.Name];
            }

            lock (_lockObjectDown)
            {
                if (!BufferDownRelations.ContainsKey(T.Name))
                {
                    object[] relationAttList = T.GetCustomAttributes(typeof(MongoRelation), false);

                    var downRelations = relationAttList.Where(relationAtt => relationAtt != null && ((MongoRelation)relationAtt).UpRelation).Cast<MongoRelation>().ToList();

                    BufferDownRelations.Add(T.Name, downRelations);
                }

                return BufferDownRelations[T.Name];
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