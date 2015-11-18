using System;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
    public class Events<T> : IEvents<T>
    {
        #region Constants and Fields

        private static IEvents<T> _events;

        #endregion

        #region Constructors and Destructors

        private Events()
        {
        }

        #endregion

        #region Public Properties

        public static IEvents<T> Instance
        {
            get { return _events ?? (_events = new Events<T>()); }
        }

        #endregion

        #region Public Methods

        public void AfterDeleteDocument(
            object Sender,
            MongoMapper<T>.OnAfterDeleteEventHandler OnAfterDelete,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType)
        {
            if (OnAfterDelete != null)
            {
                OnAfterDelete(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterDelete(Sender, ClassType.Name);
            }

            if (OnAfterComplete != null)
            {
                OnAfterComplete(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(Sender, ClassType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Delete(Sender, ClassType.Name);
            }
        }

        public void AfterInsertDocument(
            object Sender,
            MongoMapper<T>.OnAfterInsertEventHandler OnAfterInsert,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType)
        {
            if (OnAfterInsert != null)
            {
                OnAfterInsert(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterInsert(Sender, ClassType.Name);
            }

            if (OnAfterComplete != null)
            {
                OnAfterComplete(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(Sender, ClassType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Insert(Sender, ClassType.Name);
            }
        }

        public void AfterUpdateDocument(
            object Sender,
            MongoMapper<T>.OnAfterModifyEventHandler OnAfterModify,
            MongoMapper<T>.OnAfterCompleteEventHandler OnAfterComplete,
            Type ClassType)
        {
            if (OnAfterModify != null)
            {
                OnAfterModify(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterModify(Sender, ClassType.Name);
            }

            if (OnAfterComplete != null)
            {
                OnAfterComplete(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(Sender, ClassType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Update(Sender, ClassType.Name);
            }
        }

        public void BeforeDeleteDocument(
            object Sender, MongoMapper<T>.OnBeforeDeleteEventHandler OnBeforeDelete, Type ClassType)
        {
            if (OnBeforeDelete != null)
            {
                OnBeforeDelete(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeDelete(Sender, ClassType.Name);
            }
        }

        public void BeforeInsertDocument(
            object Sender, MongoMapper<T>.OnBeforeInsertEventHandler OnBeforeInsert, Type ClassType)
        {
            if (OnBeforeInsert != null)
            {
                OnBeforeInsert(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeInsert(Sender, ClassType.Name);
            }
        }

        public void BeforeUpdateDocument(
            object Sender, MongoMapper<T>.OnBeforeModifyEventHandler OnBeforeModify, Type ClassType)
        {
            if (OnBeforeModify != null)
            {
                OnBeforeModify(Sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeModify(Sender, ClassType.Name);
            }
        }

        public void ObjectInit(object Sender, MongoMapper<T>.OnObjectInitEventHandler OnObjectInit, Type ClassType)
        {
            if (OnObjectInit != null)
            {
                OnObjectInit(Sender, new EventArgs());
            }
        }

        public void ObjectComplete(object Sender, MongoMapper<T>.OnObjectCompleteEventHandler OnObjectComplete, Type ClassType)
        {
            if (OnObjectComplete != null)
            {
                OnObjectComplete(Sender, new EventArgs());
            }

        }

        #endregion
    }
}