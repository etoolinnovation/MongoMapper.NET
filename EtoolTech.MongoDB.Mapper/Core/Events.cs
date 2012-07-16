namespace EtoolTech.MongoDB.Mapper
{
    using System;

    using EtoolTech.MongoDB.Mapper.Interfaces;

    public class Events : IEvents
    {
        #region Constants and Fields

        private static IEvents _events;

        #endregion

        #region Constructors and Destructors

        private Events()
        {
        }

        #endregion

        #region Public Properties

        public static IEvents Instance
        {
            get
            {
                return _events ?? (_events = new Events());
            }
        }

        #endregion

        #region Public Methods

        public void AfterDeleteDocument(
            object sender,
            MongoMapper.OnAfterDeleteEventHandler onAfterDelete,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType)
        {
            if (onAfterDelete != null)
            {
                onAfterDelete(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterDelete(sender, classType.Name);
            }

            if (onAfterComplete != null)
            {
                onAfterComplete(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(sender, classType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Delete(sender, classType.Name);
            }
        }

        public void AfterInsertDocument(
            object sender,
            MongoMapper.OnAfterInsertEventHandler onAfterInsert,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType)
        {
            if (onAfterInsert != null)
            {
                onAfterInsert(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterInsert(sender, classType.Name);
            }

            if (onAfterComplete != null)
            {
                onAfterComplete(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(sender, classType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Insert(sender, classType.Name);
            }
        }

        public void AfterUpdateDocument(
            object sender,
            MongoMapper.OnAfterModifyEventHandler onAfterModify,
            MongoMapper.OnAfterCompleteEventHandler onAfterComplete,
            Type classType)
        {
            if (onAfterModify != null)
            {
                onAfterModify(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterModify(sender, classType.Name);
            }

            if (onAfterComplete != null)
            {
                onAfterComplete(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnAfterComplete(sender, classType.Name);
            }

            if (CustomContext.CacheManager != null)
            {
                CustomContext.CacheManager.Update(sender, classType.Name);
            }
        }

        public void BeforeDeleteDocument(
            object sender, MongoMapper.OnBeforeDeleteEventHandler onBeforeDelete, Type classType)
        {
            if (onBeforeDelete != null)
            {
                onBeforeDelete(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeDelete(sender, classType.Name);
            }
        }

        public void BeforeInsertDocument(
            object sender, MongoMapper.OnBeforeInsertEventHandler onBeforeInsert, Type classType)
        {
            if (onBeforeInsert != null)
            {
                onBeforeInsert(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeInsert(sender, classType.Name);
            }
        }

        public void BeforeUpdateDocument(
            object sender, MongoMapper.OnBeforeModifyEventHandler onBeforeModify, Type classType)
        {
            if (onBeforeModify != null)
            {
                onBeforeModify(sender, new EventArgs());
            }

            if (CustomContext.Rules != null)
            {
                CustomContext.Rules.OnBeforeModify(sender, classType.Name);
            }
        }

        #endregion
    }
}