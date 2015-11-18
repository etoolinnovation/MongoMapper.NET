using System;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IEvents<T>
    {
        #region Public Methods

        void AfterDeleteDocument(
            object Sender,
            MongoMapper<T>.OnAfterDeleteEventHandler OnAfterDelete,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void AfterInsertDocument(
            object Sender,
            MongoMapper<T>.OnAfterInsertEventHandler OnAfterInsert,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void AfterUpdateDocument(
            object Sender,
            MongoMapper<T>.OnAfterModifyEventHandler OnAfterModify,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void BeforeDeleteDocument(object Sender, MongoMapper<T>.OnBeforeDeleteEventHandler OnBeforeDelete, Type ClassType);

        void BeforeInsertDocument(object Sender, MongoMapper<T>.OnBeforeInsertEventHandler OnBeforeInsert, Type ClassType);

        void BeforeUpdateDocument(object Sender, MongoMapper<T>.OnBeforeModifyEventHandler OnBeforeModify, Type ClassType);

        void ObjectInit(object Sender, MongoMapper<T>.OnObjectInitEventHandler OnObjectInit, Type ClassType);

        void ObjectComplete(object Sender, MongoMapper<T>.OnObjectCompleteEventHandler OnObjectComplete, Type ClassType);

        #endregion
    }
}