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
                CheckRelation(Sender, relation, false);
            }
        }

        public void EnsureUpRelations(object Sender, Type ClassType, IFinder Finder)
        {
            foreach (MongoRelation relation in GetUpRelations(ClassType))
            {
                CheckRelation(Sender, relation, true);
            }
        }

        private static void CheckRelation(object Sender, MongoRelation Relation, bool FromUp)
        {
                     
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();

            for (int index = 0; index < Relation.CurrentFieldNames.Length; index++)
            {
                string currentFieldName = Relation.CurrentFieldNames[index];
                string relationFieldName = Relation.RelationFieldNames[index];                
                object v = ReflectionUtility.GetPropertyValue(Sender, currentFieldName);
                fieldValues.Add(relationFieldName, v);
            }

            if (fieldValues.All(V => V.Value == null))
                return;


            var filters = fieldValues.Select(CurrentFieldvalue => MongoQuery<BsonDocument>.Eq(Relation.RelationObjectName, CurrentFieldvalue.Key, CurrentFieldvalue.Value)).ToList();

            var relationCollection = CollectionsManager.GetCollection<BsonDocument>(Relation.RelationObjectName);

            var documentCount = relationCollection.CountAsync(Builders<BsonDocument>.Filter.And(filters)).Result;

            bool okRelation = FromUp ? documentCount != 0 : documentCount == 0;

            if (!okRelation)
            {
                if (FromUp)
                    throw new ValidateUpRelationException(string.Join(",",filters.FilterToJson()));

                throw new ValidateDownRelationException(string.Join(",", filters.FilterToJson()));
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

                    var downRelations = relationAttList.Where(RelationAtt => RelationAtt != null && !((MongoRelation) RelationAtt).UpRelation).Cast<MongoRelation>().ToList();

                    BufferDownRelations.Add(T.Name, downRelations);
                }

                return BufferDownRelations[T.Name];
            }
        }

        public List<MongoRelation> GetUpRelations(Type T)
        {

            if (BufferUpRelations.ContainsKey(T.Name))
            {
                return BufferUpRelations[T.Name];
            }

            lock (_lockObjectDown)
            {
                if (!BufferUpRelations.ContainsKey(T.Name))
                {
                    object[] relationAttList = T.GetCustomAttributes(typeof(MongoRelation), false);

                    var upRelations = relationAttList.Where(RelationAtt => RelationAtt != null && ((MongoRelation)RelationAtt).UpRelation).Cast<MongoRelation>().ToList();

                    BufferUpRelations.Add(T.Name, upRelations);
                }

                return BufferUpRelations[T.Name];
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