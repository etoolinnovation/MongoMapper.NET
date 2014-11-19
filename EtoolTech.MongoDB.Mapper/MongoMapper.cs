using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
                                        IMongoMapperVersionable,
                                        ISupportInitialize
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
            MongoMapperHelper.RebuildClass(_classType, false);
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

        public delegate void OnObjectInitEventHandler(object sender, EventArgs e);

        public delegate void OnObjectCompleteEventHandler(object sender, EventArgs e);

        #endregion

        #region Public Events

        public event OnAfterCompleteEventHandler OnAfterComplete;

        public event OnAfterDeleteEventHandler OnAfterDelete;

        public event OnAfterInsertEventHandler OnAfterInsert;

        public event OnAfterModifyEventHandler OnAfterModify;

        public event OnBeforeDeleteEventHandler OnBeforeDelete;

        public event OnBeforeInsertEventHandler OnBeforeInsert;

        public event OnBeforeModifyEventHandler OnBeforeModify;

        public event OnObjectInitEventHandler OnObjectInit;

        public event OnObjectCompleteEventHandler OnObjectComplete;

        #endregion

        #region Public Properties

        #region IMongoMapperIdeable Members

        [BsonId(IdGenerator = typeof (MongoMapperIdGenerator))]
        [XmlIgnore]
        public long m_id { get; set; }

        #endregion

        #region IMongoMapperVersionable Members

        [XmlIgnore]
        public long m_dv { get; set; }

        public bool IsLastVersion()
        {
            return IsLastVersion(false);
        }

        public bool IsLastVersion(bool Force)
        {
            if (!Force && String.IsNullOrEmpty(ConfigManager.GetClientSettings(_classType.Name).ReplicaSetName))
                return true;

            if (m_id == default(long))
            {
                m_id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", m_id);

            MongoCursor result =
                CollectionsManager.GetPrimaryCollection(_classType.Name).FindAs(_classType, query).SetFields(
                    Fields.Include("m_dv"));

            return ((IMongoMapperVersionable) result.Cast<object>().First()).m_dv == m_dv;
        }

        public void FillFromLastVersion()
        {
            FillFromLastVersion(false);
        }

        public void FillFromLastVersion(bool Force)
        {
            if (!Force && String.IsNullOrEmpty(ConfigManager.GetClientSettings(_classType.Name).ReplicaSetName)) return;

            if (m_id == default(long))
            {
                m_id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", m_id);

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

        [BsonExtraElements]
        public IDictionary<string, object> ExtraElements { get; set; }

        #endregion

        #region Public Methods

        #region IMongoMapperOriginable Members

        public T GetOriginalObject<T>()
        {
            if (!ConfigManager.EnableOriginalObject(_classType.Name))
            {
                throw new NotImplementedException("This functionality is disabled, enable it in the App.config");
            }

            if (m_id == default(long) || OriginalObject == null || OriginalObject.Length == 0)
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
            if (OriginalIsEmpty(Force) && m_id != default(long) &&
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

        public void Delete()
        {
            Events.BeforeDeleteDocument(this, OnBeforeDelete, _classType);

            EnsureDownRelations();

            DeleteDocument();

            Events.AfterDeleteDocument(this, OnAfterDelete, OnAfterComplete, _classType);
        }

        public void DeleteDocument()
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

        public int Save()
        {
            int result = -1;

            PropertyValidator.Validate(this, _classType);

            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(_classType.Name)*1024*1024;

            ChildsManager.ManageChilds(this);

            if (m_id == default(long))
            {
                long id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
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
                UpdateDocument(m_id);
                result = 1;
            }

            //Si salvan y no se pide el objeto otra vez la string queda vacia, la llenamos aqui
            //TODO: No estoy muy convencido de esto revisar ...                
            SaveOriginal();
            return result;
        }

        public void ServerUpdate(UpdateBuilder Update, bool Refill = true)
        {                                    
            if (m_id == default(long))
            {
                m_id = Finder.Instance.FindIdByKey(_classType, GetPrimaryKeyValues());
            }
            IMongoQuery query = Query.EQ("_id", m_id);

            var args = new FindAndModifyArgs()
            {
                Query = query,
                SortBy = null,
                Update = Update,
                VersionReturned = Refill ? FindAndModifyDocumentVersion.Modified : FindAndModifyDocumentVersion.Original
            };

            FindAndModifyResult result = CollectionsManager.GetCollection(_classType.Name).FindAndModify(args);


            if (!String.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new ServerUpdateException(result.ErrorMessage);
            }


            if (Refill)
            {
                ReflectionUtility.CopyObject(result.GetModifiedDocumentAs(_classType), this);
            }
        }

        public void UpdateDocument(long Id)
        {
            Events.BeforeUpdateDocument(this, OnBeforeModify, _classType);

            EnsureUpRelations();

            m_id = Id;

            Writer.Instance.Update(_classType.Name, _classType, this);

            Events.AfterUpdateDocument(this, OnAfterModify, OnAfterComplete, _classType);
        }

        #endregion

   

        public static T FindByKey<T>(params object[] values)
        {
            return Finder.Instance.FindByKey<T>(values);
        }

        public static MongoCollection GetCollection<T>()
        {
            return CollectionsManager.GetCollection(typeof (T).Name);
        }

		public static IEnumerable<BsonDocument> Aggregate<T>(params BsonDocument[] operations)
        {
			AggregateArgs ars = new AggregateArgs ();
			ars.Pipeline = operations;
			ars.OutputMode = AggregateOutputMode.Cursor;

			return
                CollectionsManager.GetCollection((typeof (T).Name)).Aggregate(ars);
        }

        public static void ServerDelete<T>(IMongoQuery query)
        {
            WriteConcernResult result = CollectionsManager.GetCollection(typeof (T).Name).Remove(query);


            if (result != null && !String.IsNullOrEmpty(result.ErrorMessage))
            {
                throw new DeleteDocumentException(result.ErrorMessage);
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
            return MongoMapperHelper.GetPrimaryKey(_classType).ToDictionary(
                KeyField => KeyField, KeyField => ReflectionUtility.GetPropertyValue(this, KeyField));
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
            Events.ObjectInit(this, OnObjectInit, _classType);
        }

        public void EndInit()
        {
            var mongoMapperOriginable = this as IMongoMapperOriginable;
            (mongoMapperOriginable).SaveOriginal(false);

            Events.ObjectComplete(this, OnObjectComplete, _classType);
        }

        #endregion
    }
}