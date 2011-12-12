using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using EtoolTech.MongoDB.Mapper.Core;
using EtoolTech.MongoDB.Mapper.Interfaces;
using EtoolTech.MongoDB.Mapper.enums;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace EtoolTech.MongoDB.Mapper
{
    [Serializable]
    public abstract class MongoMapper: IBsonSerializable
    {
        #region Eventos

        #region Delegates

        public delegate void OnAfterCompleteEventHandler(object sender, EventArgs e);

        public delegate void OnAfterDeleteEventHandler(object sender, EventArgs e);

        public delegate void OnAfterInsertEventHandler(object sender, EventArgs e);

        public delegate void OnAfterModifyEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeDeleteEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeInsertEventHandler(object sender, EventArgs e);

        public delegate void OnBeforeModifyEventHandler(object sender, EventArgs e);

        #endregion

        public event OnBeforeInsertEventHandler OnBeforeInsert;

        public event OnAfterInsertEventHandler OnAfterInsert;

        public event OnBeforeModifyEventHandler OnBeforeModify;

        public event OnAfterModifyEventHandler OnAfterModify;

        public event OnBeforeDeleteEventHandler OnBeforeDelete;

        public event OnAfterDeleteEventHandler OnAfterDelete;

        public event OnAfterCompleteEventHandler OnAfterComplete;

        #endregion

        private static readonly IFinder Finder = new Finder();
        private static readonly IRelations Relations = new Relations();
        private static readonly IEvents Events = new Events();
        private readonly Dictionary<string,object> RelationBuffer = new Dictionary<string, object>();

        private readonly Type _classType;        
        private BsonDocument _originalObject;
        [BsonIgnore] internal bool RepairCollection;

        [BsonId]
        [XmlIgnore]
        public Guid MongoMapper_Id { get; set; }

        //TODO: Pendiente temas de transacciones
        //public bool Commited;
        //public CommitOperation CommitOp;
        //public string TransactionID;

      
        protected MongoMapper()
        {
            _classType = GetType();
            Helper.RebuildClass(_classType, RepairCollection);          
        }

        public static IQueryable<T> QueryContext<T>()
        {
            return Helper.GetCollection<T>(typeof(T).Name).AsQueryable<T>();            
        }
      
        public List<T> GetRelation<T>(string relation)
        {
            if (!RelationBuffer.ContainsKey(relation))
            {
                RelationBuffer.Add(relation,Relations.GetRelation<T>(this, relation, _classType, Finder));
            }
            return (List<T>)RelationBuffer[relation];
        }


        private void EnsureUpRelations()
        {
            Relations.EnsureUpRelations(this, _classType, Finder);
        }

        private void EnsureDownRelations()
        {
            Relations.EnsureDownRelations(this, _classType, Finder);
        }

        private Dictionary<string, object> GetKeyValues()
        {
            var result = new Dictionary<string, object>();
            foreach (string keyField in Helper.GetPrimaryKey(_classType))
            {
                PropertyInfo propertyInfo = _classType.GetProperty(keyField);
                result.Add(keyField, propertyInfo.GetValue(this, null));
            }
            return result;
        }

        public object GetOriginalValue<T>(string fieldName)
        {
            if (MongoMapper_Id == Guid.Empty) return null;

            if (_originalObject == null)
                _originalObject = Finder.FindBsonDocumentById<T>(MongoMapper_Id);
            return _originalObject[fieldName];
        }

        public T GetOriginalObject<T>()
        {
            if (MongoMapper_Id == Guid.Empty) return default(T);
            
            if (_originalObject == null)
                _originalObject = Finder.FindBsonDocumentById<T>(MongoMapper_Id);

            return BsonSerializer.Deserialize<T>(_originalObject);
        }

        public static List<T> AllAsList<T>()
        {
            return Finder.AllAsList<T>();
        }

        public static MongoCursor<T> AllAsCursor<T>()
        {
            return Finder.AllAsCursor<T>();
        }


        #region Write Methods

        public void Save<T>()
        {
            PropertyValidator.Validate(this, _classType);

            if (MongoMapper_Id == Guid.Empty)
            {
                Guid id = Finder.FindGuidByKey<T>(GetKeyValues());
                if (id == Guid.Empty)
                {
                    InsertDocument();
                }
                else
                {
                    UpdateDocument(id);
                }
            }
            else
            {
                UpdateDocument(MongoMapper_Id);
            }
        }

        private void UpdateDocument(Guid id)
        {
            Events.BeforeUpdateDocument(this, OnBeforeModify, _classType);

            EnsureUpRelations();

            MongoMapper_Id = id;

            SafeModeResult result = Helper.GetCollection(Helper.GetCollectioName(_classType.Name)).Save(this, SafeMode.Create(Helper.SafeMode));

            Events.AfterUpdateDocument(this, OnAfterModify, OnAfterComplete, _classType);
        }

        private void InsertDocument()
        {
            Events.BeforeInsertDocument(this, OnBeforeInsert, _classType);

            EnsureUpRelations();

            SafeModeResult result = Helper.GetCollection(Helper.GetCollectioName(_classType.Name)).Insert(this, SafeMode.Create(Helper.SafeMode));            
            Events.AfterInsertDocument(this, OnAfterInsert, OnAfterComplete, _classType);
        }


        public void Delete<T>()
        {
            Events.BeforeDeleteDocument(this, OnBeforeDelete, _classType);

            EnsureDownRelations();

            DeleteDocument<T>();

            Events.AfterDeleteDocument(this, OnAfterDelete, OnAfterComplete, _classType);
        }


        private void DeleteDocument<T>()
        {
            if (MongoMapper_Id == Guid.Empty)
            {
                MongoMapper_Id = Finder.FindGuidByKey<T>(GetKeyValues());
            }
            QueryComplete query = Query.EQ("_id", MongoMapper_Id);
            FindAndModifyResult result = Helper.GetCollection(Helper.GetCollectioName(_classType.Name)).FindAndRemove(query, null);            
        }

        #endregion

        #region FindAsList Methods

        public static T FindByKey<T>(params object[] values)
        {
            return Finder.FindByKey<T>(values);
        }
     
 
        public static List<T> FindAsList<T>(QueryComplete query)
        {
            return Finder.FindAsList<T>(query);
        }

        public static MongoCursor<T> FindAsCursor<T>(QueryComplete query = null)
        {
            return Finder.FindAsCursor<T>(query);
        }

        public static List<T> FindAsList<T>(string fieldName, object value)
        {
            return Finder.FindAsList<T>(Finder.GetEqQuery(value.GetType(),fieldName,value));
        }

        public static MongoCursor<T> FindAsCursor<T>(string fieldName, object value)
        {
            return Finder.FindAsCursor<T>(Finder.GetEqQuery(value.GetType(), fieldName, value));
        }



        public static List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {            
            return Finder.FindAsList<T>(exp);
        }

        public static MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            return Finder.FindAsCursor<T>(exp);
        }


        #endregion



        #region IBsonSerializable Members

        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            object o = BsonClassMapSerializer.Instance.Deserialize(bsonReader, nominalType, options);
            //TODO: this._originalObject = ;
            return o;
        }

        public bool GetDocumentId(out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            return BsonClassMapSerializer.Instance.GetDocumentId(this, out id, out idNominalType, out idGenerator);
        }

        public void Serialize(BsonWriter bsonWriter, Type nominalType, IBsonSerializationOptions options)
        {
            BsonClassMapSerializer.Instance.Serialize(bsonWriter, nominalType, this, options);
        }

        public void SetDocumentId(object id)
        {
            BsonClassMapSerializer.Instance.SetDocumentId(this, id);
        }

        #endregion
    }
}