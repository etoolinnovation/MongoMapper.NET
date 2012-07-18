#region



#endregion

namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using EtoolTech.MongoDB.Mapper.Configuration;    
    using EtoolTech.MongoDB.Mapper.Exceptions;
    using EtoolTech.MongoDB.Mapper.Interfaces;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Builders;

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

        private byte[] _originalObject;

        private object _tOriginalObjet;

        #endregion

        #region Constructors and Destructors

        protected MongoMapper()
        {
            this._classType = this.GetType();
            Helper.RebuildClass(this._classType, false);
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

        [BsonId(IdGenerator = typeof(MongoMapperIdGenerator))]
        [XmlIgnore]
        public long MongoMapper_Id { get; set; }

        #endregion

        #region Properties

        [BsonIgnore]
        private byte[] OriginalObject
        {
            get
            {
                return this._originalObject;
            }
            set
            {
                this._originalObject = value;
                this._tOriginalObjet = null;
            }
        }

        #endregion

        #region Public Methods

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

        public void Delete<T>()
        {
            Events.BeforeDeleteDocument(this, this.OnBeforeDelete, this._classType);

            this.EnsureDownRelations();

            this.DeleteDocument<T>();

            Events.AfterDeleteDocument(this, this.OnAfterDelete, this.OnAfterComplete, this._classType);
        }

        public void DeleteDocument<T>()
        {
            if (this.MongoMapper_Id == default(long))
            {
                this.MongoMapper_Id = Finder.Instance.FindIdByKey<T>(this.GetKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", this.MongoMapper_Id);

            SafeModeResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(this._classType.Name)).Remove(
                    query);

            //TODO: ver de devolver DeleteDocumentException
            if (ConfigManager.SafeMode(this._classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new DeleteDocumentException(result.ErrorMessage);
                }
            }
        }

        public void EnsureDownRelations()
        {
            Relations.EnsureDownRelations(this, this._classType, Finder.Instance);
        }

        public void EnsureUpRelations()
        {
            Relations.EnsureUpRelations(this, this._classType, Finder.Instance);
        }

        public T GetOriginalObject<T>()
        {
            if (!ConfigManager.EnableOriginalObject(this._classType.Name))
            {
                throw new NotImplementedException("This functionality is disabled, enable it in the App.config");
            }

            if (this.MongoMapper_Id == default(long) || this.OriginalObject == null || this.OriginalObject.Length == 0)
            {
                return (Activator.CreateInstance<T>());
            }
            return this.GetOriginalT<T>();
        }

        public T GetOriginalT<T>()
        {
            if (this._tOriginalObjet != null)
            {
                return (T)this._tOriginalObjet;
            }

            this._tOriginalObjet = ObjectSerializer.ToObjectSerialize<T>(this.OriginalObject);
            return (T)this._tOriginalObjet;
        }

        public List<T> GetRelation<T>(string relation)
        {
            if (!this._relationBuffer.ContainsKey(relation))
            {
                this._relationBuffer.Add(
                    relation, Relations.GetRelation<T>(this, relation, this._classType, Finder.Instance));
            }
            return (List<T>)this._relationBuffer[relation];
        }

        public void InsertDocument()
        {
            Events.BeforeInsertDocument(this, this.OnBeforeInsert, this._classType);

            this.EnsureUpRelations();

            SafeModeResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(this._classType.Name)).Insert(this);

            if (ConfigManager.SafeMode(this._classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            Events.AfterInsertDocument(this, this.OnAfterInsert, this.OnAfterComplete, this._classType);
        }

        public void Save<T>()
        {
            PropertyValidator.Validate(this, this._classType);

            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(this._classType.Name) * 1024 * 1024;

            ChildsManager.ManageChilds(this);

            if (this.MongoMapper_Id == default(long))
            {
                long id = Finder.Instance.FindIdByKey<T>(this.GetKeyValues());
                if (id == default(long))
                {
                    this.InsertDocument();
                }
                else
                {
                    //Si llega aqui ya existe un registro con esa key y no es el que tenemos cargado
                    if (ConfigManager.ExceptionOnDuplicateKey(this._classType.Name))
                    {
                        throw new DuplicateKeyException();
                    }

                    this.UpdateDocument(id);
                }
            }
            else
            {
                this.UpdateDocument(this.MongoMapper_Id);
            }

            //Si salvan y no se pide el objeto otra vez la string json queda vacia, la llenamos aqui
            //TODO: No estoy muy convencido de esto revisar ...                
            this.SaveOriginal();
        }

		private bool OriginalIsEmpty(bool force = false)
		{
			if (force) return true;
			return this._originalObject == null || this._originalObject.Length == 0;
		}

        public void SaveOriginal(bool force = false)
        {      
            if (OriginalIsEmpty(force) && this.MongoMapper_Id != default(long) && ConfigManager.EnableOriginalObject(this._classType.Name))
            {
                this.OriginalObject = ObjectSerializer.ToByteArray(this);
            }
        }

        public void ServerUpdate<T>(UpdateBuilder update, bool refill = true)
        {
            if (this.MongoMapper_Id == default(long))
            {
                this.MongoMapper_Id = Finder.Instance.FindIdByKey<T>(this.GetKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", this.MongoMapper_Id);

            FindAndModifyResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(this._classType.Name)).
                    FindAndModify(query, null, update, refill, true);

            //TODO: ver de devolver ServerUpdateException
            if (ConfigManager.SafeMode(this._classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new ServerUpdateException(result.ErrorMessage);
                }
            }

            if (refill)
            {
                ReflectionUtility.CopyObject(result.GetModifiedDocumentAs(typeof(T)), this);
            }
        }

        public void UpdateDocument(long id)
        {
            Events.BeforeUpdateDocument(this, this.OnBeforeModify, this._classType);

            this.EnsureUpRelations();

            this.MongoMapper_Id = id;

            SafeModeResult result =
                CollectionsManager.GetCollection(CollectionsManager.GetCollectioName(this._classType.Name)).Save(this);

            if (ConfigManager.SafeMode(this._classType.Name))
            {
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            Events.AfterUpdateDocument(this, this.OnAfterModify, this.OnAfterComplete, this._classType);
        }

        #endregion

        #region Methods

        private Dictionary<string, object> GetKeyValues()
        {
            return Helper.GetPrimaryKey(this._classType).ToDictionary(
                keyField => keyField, keyField => ReflectionUtility.GetPropertyValue(this, keyField));
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}