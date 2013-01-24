using System;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IEvents
    {
        #region Public Methods

        void AfterDeleteDocument(
            object Sender,
            MongoMapper.OnAfterDeleteEventHandler OnAfterDelete,
            MongoMapper.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void AfterInsertDocument(
            object Sender,
            MongoMapper.OnAfterInsertEventHandler OnAfterInsert,
            MongoMapper.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void AfterUpdateDocument(
            object Sender,
            MongoMapper.OnAfterModifyEventHandler OnAfterModify,
            MongoMapper.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType);

        void BeforeDeleteDocument(object Sender, MongoMapper.OnBeforeDeleteEventHandler OnBeforeDelete, Type ClassType);

        void BeforeInsertDocument(object Sender, MongoMapper.OnBeforeInsertEventHandler OnBeforeInsert, Type ClassType);

        void BeforeUpdateDocument(object Sender, MongoMapper.OnBeforeModifyEventHandler OnBeforeModify, Type ClassType);

        #endregion
    }
}