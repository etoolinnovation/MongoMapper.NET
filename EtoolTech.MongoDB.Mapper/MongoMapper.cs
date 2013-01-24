using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    [Serializable]
    public abstract class MongoMapper : IMongoMapperOriginable,
                                        IMongoMapperRelationable,
                                        IMongoMapperWriteable,
                                        IMongoMapperIdeable,
                                        IMongoMapperVersionable
    {
        #region Constants and Fields

        private static readonly IChildsManager ChildsManager = Mapper.ChildsManager.Instance;

        private static readonly IEvents Events = Mapper.Events.Instance;

        private static readonly IRelations Relations = Mapper.Relations.Instance;

        private readonly Type _classType;

        private readonly Dictionary<string, object> _relationBuffer = new Dictionary<string, object>();

        private byte[] _originalObject;

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

        [BsonId(IdGenerator = typeof (MongoMapperIdGenerator))]
        [XmlIgnore]
        public long MongoMapper_Id { get; set; }

        #endregion

        #region IMongoMapperVersionable Members

        [XmlIgnore]
        public long MongoMapperDocumentVersion { get; set; }

        public bool IsLastVersion()
        {
            return IsLastVersion(false);
        }

        public bool IsLastVersion(bool Force)
        {
            if (!Force && String.IsNullOrEmpty(MongoMapperConfiguration.GetConfig().Server.ReplicaSetName)) return true;

            if (MongoMapper_Id == default(long))
            {
                MongoMapper_Id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", MongoMapper_Id);

            MongoCursor result =
                CollectionsManager.GetPrimaryCollection(_classType.Name).FindAs(_classType, query).SetFields(
                    Fields.Include("MongoMapperDocumentVersion"));

            return ((IMongoMapperVersionable) result.Cast<object>().First()).MongoMapperDocumentVersion ==
                   MongoMapperDocumentVersion;
        }

        public void FillFromLastVersion()
        {
            FillFromLastVersion(false);
        }

        public void FillFromLastVersion(bool Force)
        {
            if (!Force && String.IsNullOrEmpty(MongoMapperConfiguration.GetConfig().Server.ReplicaSetName)) return;

            if (MongoMapper_Id == default(long))
            {
                MongoMapper_Id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", MongoMapper_Id);

            MongoCursor result = CollectionsManager.GetPrimaryCollection(_classType.Name).FindAs(_classType, query);

            ReflectionUtility.CopyObject(result.Cast<object>().First(), this);
        }

        #endregion

        #endregion

        #region Properties

        [BsonIgnore]
        private byte[] OriginalObject
        {
            get { return _originalObject; }
            set
            {
                _originalObject = value;
                _tOriginalObjet = null;
            }
        }

        #endregion

        #region Public Methods

        #region IMongoMapperOriginable Members

        public T GetOriginalObject<T>()
        {
            if (!ConfigManager.EnableOriginalObject(_classType.Name))
            {
                throw new NotImplementedException("This functionality is disabled, enable it in the App.config");
            }

            if (MongoMapper_Id == default(long) || OriginalObject == null || OriginalObject.Length == 0)
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

            _tOriginalObjet = ObjectSerializer.ToObjectSerialize<T>(OriginalObject);
            return (T) _tOriginalObjet;
        }

        public void SaveOriginal(bool Force = false)
        {
            if (OriginalIsEmpty(Force) && MongoMapper_Id != default(long) &&
                ConfigManager.EnableOriginalObject(_classType.Name))
            {
                OriginalObject = ObjectSerializer.ToByteArray(this);
            }
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

        public List<T> GetRelation<T>(string Relation)
        {
            if (!_relationBuffer.ContainsKey(Relation))
            {
                _relationBuffer.Add(
                    Relation, Relations.GetRelation<T>(this, Relation, _classType, Finder.Instance));
            }
            return (List<T>) _relationBuffer[Relation];
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
            Writer.Instance.Delete(_classType.Name, _classType, this);
        }

        public void InsertDocument()
        {
            Events.BeforeInsertDocument(this, OnBeforeInsert, _classType);

            EnsureUpRelations();

            Writer.Instance.Insert(_classType.Name, _classType, this);

            Events.AfterInsertDocument(this, OnAfterInsert, OnAfterComplete, _classType);
        }

        public int Save<T>()
        {
            int result = -1;

            PropertyValidator.Validate(this, _classType);

            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(_classType.Name)*1024*1024;

            ChildsManager.ManageChilds(this);

            if (MongoMapper_Id == default(long))
            {
                long id = Finder.Instance.FindIdByKey<T>(GetPrimaryKeyValues());
                if (id == default(long))
                {
                    InsertDocument();
                    result = 0;
                }
                else
                {
                    //Si llega aqui ya existe un registro con esa key y no es el que tenemos cargado
                    if (ConfigManager.ExceptionOnDuplicateKey(_classType.Name))
                    {
                        throw new DuplicateKeyException();
                    }

                    UpdateDocument(id);
                    result = 1;
                }
            }
            else
            {
                UpdateDocument(MongoMapper_Id);
                result = 1;
            }

            //Si salvan y no se pide el objeto otra vez la string queda vacia, la llenamos aqui
            //TODO: No estoy muy convencido de esto revisar ...                
            SaveOriginal();
            return result;
        }

        public void ServerUpdate<T>(UpdateBuilder Update, bool Refill = true)
        {
            if (MongoMapper_Id == default(long))
            {
                MongoMapper_Id = Finder.Instance.FindIdByKey<T>(GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", MongoMapper_Id);

            FindAndModifyResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(_classType.Name)).
                    FindAndModify(query, null, Update, Refill, true);

            if (ConfigManager.SafeMode(_classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new ServerUpdateException(result.ErrorMessage);
                }
            }

            if (Refill)
            {
                ReflectionUtility.CopyObject(result.GetModifiedDocumentAs(typeof (T)), this);
            }
        }

        public void UpdateDocument(long Id)
        {
            Events.BeforeUpdateDocument(this, OnBeforeModify, _classType);

            EnsureUpRelations();

            MongoMapper_Id = Id;

            Writer.Instance.Update(_classType.Name, _classType, this);

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

        public static MongoCollection GetCollection<T>()
        {
            return CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(typeof (T).Name));
        }

        public static AggregateResult Aggregate<T>(params BsonDocument[] operations)
        {
            return
                CollectionsManager.GetCollection(
                    CollectionsManager.GetCollectioName(typeof (T).Name)).Aggregate(operations);
        }

        public static void ServerDelete<T>(IMongoQuery query)
        {
            WriteConcernResult result = CollectionsManager.GetCollection(
                CollectionsManager.GetCollectioName(typeof (T).Name)).Remove(query);

            if (ConfigManager.SafeMode(typeof (T).Name))
            {
                if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new DeleteDocumentException(result.ErrorMessage);
                }
            }
        }

        private bool OriginalIsEmpty(bool force = false)
        {
            if (force) return true;
            return _originalObject == null || _originalObject.Length == 0;
        }

        #endregion

        #region Methods

        private Dictionary<string, object> GetPrimaryKeyValues()
        {
            return Helper.GetPrimaryKey(_classType).ToDictionary(
                KeyField => KeyField, KeyField => ReflectionUtility.GetPropertyValue(this, KeyField));
        }

        #endregion
    }
}