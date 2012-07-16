namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    using System;

    public interface IEvents
    {
        #region Public Methods

        void AfterDeleteDocument(
            object sender,
            MongoMapper.OnAfterDeleteEventHandler onAfterDelete,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType);

        void AfterInsertDocument(
            object sender,
            MongoMapper.OnAfterInsertEventHandler onAfterInsert,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType);

        void AfterUpdateDocument(
            object sender,
            MongoMapper.OnAfterModifyEventHandler onAfterModify,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType);

        void BeforeDeleteDocument(object sender, MongoMapper.OnBeforeDeleteEventHandler onBeforeDelete, Type classType);

        void BeforeInsertDocument(object sender, MongoMapper.OnBeforeInsertEventHandler onBeforeInsert, Type classType);

        void BeforeUpdateDocument(object sender, MongoMapper.OnBeforeModifyEventHandler onBeforeModify, Type classType);

        #endregion
    }
}