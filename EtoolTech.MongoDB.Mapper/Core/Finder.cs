namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using EtoolTech.MongoDB.Mapper.Configuration;
    using EtoolTech.MongoDB.Mapper.Exceptions;
    using EtoolTech.MongoDB.Mapper.Interfaces;

    using global::MongoDB.Bson;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

    public class Finder : IFinder
    {
        #region Constructors and Destructors

        private Finder()
        {
        }

        #endregion

        #region Properties

        internal static IFinder Instance
        {
            get
            {
                return new Finder();
            }
        }

        #endregion

        #region Public Methods

        public MongoCursor<T> AllAsCursor<T>()
        {
            MongoCursor<T> result = CollectionsManager.GetCollection(typeof(T).Name).FindAllAs<T>();

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine("{}");
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;
        }

        public List<T> AllAsList<T>()
        {
            List<T> list = this.AllAsCursor<T>().ToList();
            foreach (T result in list)
            {
                SaveOriginal(result);
            }
            return list;
        }

        public MongoCursor<T> FindAsCursor<T>(IMongoQuery query = null)
        {
            if (query == null)
            {
                return CollectionsManager.GetCollection(typeof(T).Name).FindAllAs<T>();
            }

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;
        }

        public MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            var ep = new ExpressionParser();
            IMongoQuery query = ep.ParseExpression(exp);

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;
        }

        public List<T> FindAsList<T>(IMongoQuery query = null)
        {
            List<T> list = FindAsCursor<T>(query).ToList();
            foreach (T result in list)
            {
                SaveOriginal(result);
            }
            return list;
        }

        public List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {
            List<T> list = FindAsCursor(exp).ToList();
            foreach (T result in list)
            {
                SaveOriginal(result);
            }
            return list;
        }

        public BsonDocument FindBsonDocumentById<T>(long id)
        {
            var result = CollectionsManager.GetCollection(typeof(T).Name).FindOneByIdAs<T>(id);
            return result.ToBsonDocument();
        }

        public T FindById<T>(long id)
        {
            var result = CollectionsManager.GetCollection(typeof(T).Name).FindOneByIdAs<T>(id);
            SaveOriginal(result);
            return result;
        }

        public object FindById(Type t, long id)
        {
            object result = CollectionsManager.GetCollection(t.Name).FindOneByIdAs(t, id);
            SaveOriginal(result);
            return result;
        }

        public T FindByKey<T>(params object[] values)
        {
            List<string> fields = Helper.GetPrimaryKey(typeof(T)).ToList();
            var keyValues = new Dictionary<string, object>();
            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i].ToUpper() == "MongoMapper_id".ToUpper() ? "_id" : fields[i];
                keyValues.Add(field, values[i]);
            }

            return this.FindObjectByKey<T>(keyValues);
        }

        public long FindIdByKey<T>(Dictionary<string, object> keyValues)
        {
            //Si la key es la interna y vieb
            if (keyValues.Count == 1 && keyValues.First().Key == "MongoMapper_Id" && keyValues.First().Value is long
                && (long)keyValues.First().Value == default(long))
            {
                return default(long);
            }

            IMongoQuery query =
                Query.And(keyValues.Select(keyValue => MongoQuery.Eq(keyValue.Key, keyValue.Value)).ToArray());

            MongoCursor<T> result =
                CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(query).SetFields(Fields.Include("_id"));

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                return default(long);
            }

            return ((IMongoMapperIdeable)result.First()).MongoMapper_Id;
        }

        public T FindObjectByKey<T>(Dictionary<string, object> keyValues)
        {
            IMongoQuery query =
                Query.And(keyValues.Select(keyValue => MongoQuery.Eq(keyValue.Key, keyValue.Value)).ToArray());

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof(T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                throw new FindByKeyNotFoundException();
            }
            T o = result.First();
            SaveOriginal(o);
            return o;
        }

        #endregion

        #region Methods

        private static void SaveOriginal<T>(T result)
        {
            var mongoMapperOriginable = result as IMongoMapperOriginable;
            if (mongoMapperOriginable != null)
            {
                (mongoMapperOriginable).SaveOriginal();
            }
        }

        #endregion
    }
}