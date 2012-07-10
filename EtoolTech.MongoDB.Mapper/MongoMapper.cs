#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Core;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

#endregion

namespace EtoolTech.MongoDB.Mapper
{
    [Serializable]
    public abstract class MongoMapper : IMongoMapperOriginable,
                                        IMongoMapperRelationable,
                                        IMongoMapperWriteable,
                                        IMongoMapperIdeable
    {
                
        
        #region Constants and Fields

        private static readonly IChildsManager ChildsManager = Mapper.ChildsManager.Instance;

        private static readonly IEvents Events = Mapper.Events.Instance;

        private static readonly IRelations Relations = Mapper.Relations.Instance;

        private readonly Type _classType;

        private readonly Dictionary<string, object> _relationBuffer = new Dictionary<string, object>();

        private string _stringOriginalObject;

        private object _tOriginalObjet;

        #endregion

        #region Constructors and Destructors

        protected MongoMapper()
        {
            _classType = GetType();
            Helper.RebuildClass(_classType, false);
        }

        #endregion

        #region Delegates

        public delegate void OnAfterCompleteEventHandler(object sender, EventArgs e);

        public delegate void OnAfterDeleteEventHandler(object sender, EventArgs e);

        public delegate void OnAfterInsertEventHandler(object sender, EventArgs e);

        public delegate void OnAfterModifyEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeDeleteEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeInsertEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeModifyEventHandler(object sender, EventArgs e);

        #endregion

        #region Public Events

        public event OnAfterCompleteEventHandler OnAfterComplete;

        public event OnAfterDeleteEventHandler OnAfterDelete;

        public event OnAfterInsertEventHandler OnAfterInsert;

        public event OnAfterModifyEventHandler OnAfterModify;

        public event OnBeforeDeleteEventHandler OnBeforeDelete;

        public event OnBeforeInsertEventHandler OnBeforeInsert;

        public event OnBeforeModifyEventHandler OnBeforeModify;

        #endregion

        #region Public Properties

        #region IMongoMapperIdeable Members

        [BsonId(IdGenerator = typeof (MongoMapperIdGenerator)), XmlIgnore]
        public long MongoMapper_Id { get; set; }

        #endregion

        #region IMongoMapperOriginable Members


        [BsonIgnore]
        private string StringOriginalObject
        {
            get
            {              
                return _stringOriginalObject;
            }
            set
            {
                _stringOriginalObject = value;
                _tOriginalObjet = null;
            }
        }

        #endregion

        #endregion

        #region Public Methods

        #region IMongoMapperOriginable Members
    
        public void SaveOriginal()
        {
                        
            if (String.IsNullOrEmpty(_stringOriginalObject) && MongoMapper_Id != default(long) &&
                ConfigManager.EnableOriginalObject(this._classType.Name))
            {
                StringOriginalObject = ObjectSerializer.SerializeToString(this);
            }
        }

        public T GetOriginalObject<T>()
        {
            if (!ConfigManager.EnableOriginalObject(_classType.Name))
            {
                throw new NotImplementedException("This functionality is disabled, enable it in the App.config");
            }

            if (MongoMapper_Id == default(long) || string.IsNullOrEmpty(StringOriginalObject))
            {
                return (Activator.CreateInstance<T>());
            }
            return GetOriginalT<T>();
        }

        public T GetOriginalT<T>()
        {
            if (_tOriginalObjet != null)
            {
                return (T) _tOriginalObjet;
            }

            _tOriginalObjet = ObjectSerializer.DeserializeFromString<T>(StringOriginalObject);
            return (T) _tOriginalObjet;
        }

        #endregion

        #region IMongoMapperRelationable Members

        public void EnsureDownRelations()
        {
            Relations.EnsureDownRelations(this, _classType, Finder.Instance);
        }

        public void EnsureUpRelations()
        {
            Relations.EnsureUpRelations(this, _classType, Finder.Instance);
        }

        public List<T> GetRelation<T>(string relation)
        {
            if (!_relationBuffer.ContainsKey(relation))
            {
                _relationBuffer.Add(
                    relation, Relations.GetRelation<T>(this, relation, _classType, Finder.Instance));
            }
            return (List<T>) _relationBuffer[relation];
        }

        #endregion

        #region IMongoMapperWriteable Members

        public void Delete<T>()
        {
            Events.BeforeDeleteDocument(this, OnBeforeDelete, _classType);

            EnsureDownRelations();

            DeleteDocument<T>();

            Events.AfterDeleteDocument(this, OnAfterDelete, OnAfterComplete, _classType);
        }

        public void DeleteDocument<T>()
        {
            if (MongoMapper_Id == default(long))
            {
                MongoMapper_Id = Finder.Instance.FindIdByKey<T>(GetKeyValues());
            }
            var query = Query.EQ("_id", MongoMapper_Id);

            var result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(_classType.Name)).Remove(
                    query);

            //TODO: ver de devolver DeleteDocumentException
            if (ConfigManager.SafeMode(_classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
        }

        public void InsertDocument()
        {
            Events.BeforeInsertDocument(this, OnBeforeInsert, _classType);

            EnsureUpRelations();

            var result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(_classType.Name)).Insert(this);

            if (ConfigManager.SafeMode(_classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            Events.AfterInsertDocument(this, OnAfterInsert, OnAfterComplete, _classType);
        }

        public void Save<T>()
        {
            PropertyValidator.Validate(this, _classType);

            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(_classType.Name)*1024*1024;

            ChildsManager.ManageChilds(this);

            if (MongoMapper_Id == default(long))
            {
                var id = Finder.Instance.FindIdByKey<T>(GetKeyValues());
                if (id == default(long))
                {
                    InsertDocument();
                }
                else
                {
                    //Si llega aqui ya existe un registro con esa key y no es el que tenemos cargado
                    if (ConfigManager.ExceptionOnDuplicateKey(_classType.Name))
                    {
                        throw new DuplicateKeyException();
                    }

                    UpdateDocument(id);
                }

            }
            else
            {
                UpdateDocument(MongoMapper_Id);
            }

            //Si salvan y no se pide el objeto otra vez la string json queda vacia, la llenamos aqui
            //TODO: No estoy muy convencido de esto revisar ...                
            SaveOriginal();

        }


        public void ServerUpdate<T>(UpdateBuilder update, bool refill = true)
        {
            if (MongoMapper_Id == default(long))
            {
                MongoMapper_Id = Finder.Instance.FindIdByKey<T>(GetKeyValues());
            }
            var query = Query.EQ("_id", MongoMapper_Id);

            var result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(_classType.Name)).
                    FindAndModify(query, null, update, refill, true);

            //TODO: ver de devolver ServerUpdateException
            if (ConfigManager.SafeMode(_classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            if (refill)
            {
                ReflectionUtility.CopyObject(result.GetModifiedDocumentAs(typeof (T)), this);
            }
        }

        public void UpdateDocument(long id)
        {
            Events.BeforeUpdateDocument(this, OnBeforeModify, _classType);

            EnsureUpRelations();

            MongoMapper_Id = id;

            var result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(_classType.Name)).Save(this);

            if (ConfigManager.SafeMode(_classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            Events.AfterUpdateDocument(this, OnAfterModify, OnAfterComplete, _classType);
        }

        #endregion

        public static MongoCursor<T> AllAsCursor<T>()
        {
            return Finder.Instance.AllAsCursor<T>();
        }

        public static List<T> AllAsList<T>()
        {
            return Finder.Instance.AllAsList<T>();
        }

        public static MongoCursor<T> FindAsCursor<T>(IMongoQuery query = null)
        {
            return Finder.Instance.FindAsCursor<T>(query);
        }

        public static MongoCursor<T> FindAsCursor<T>(string fieldName, object value)
        {
            return Finder.Instance.FindAsCursor<T>(MongoQuery.Eq(fieldName, value));
        }

        public static MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            return Finder.Instance.FindAsCursor(exp);
        }

        public static List<T> FindAsList<T>(IMongoQuery query)
        {
            return Finder.Instance.FindAsList<T>(query);
        }

        public static List<T> FindAsList<T>(string fieldName, object value)
        {
            return Finder.Instance.FindAsList<T>(MongoQuery.Eq(fieldName, value));
        }

        public static List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {
            return Finder.Instance.FindAsList(exp);
        }

        public static T FindByKey<T>(params object[] values)
        {
            return Finder.Instance.FindByKey<T>(values);
        }

        #endregion

        #region Methods

        private Dictionary<string, object> GetKeyValues()
        {
            return Helper.GetPrimaryKey(_classType).ToDictionary(
                keyField => keyField, keyField => ReflectionUtility.GetPropertyValue(this, keyField));
        }

        #endregion

     
    }
}