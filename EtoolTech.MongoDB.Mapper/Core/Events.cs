using System;
using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class Events : IEvents
    {
        #region IEvents Members

        public void AfterInsertDocument(object sender, MongoMapper.OnAfterInsertEventHandler onAfterInsert,
                                        MongoMapper.OnAfterCompleteEventHandler onAfterComplete, Type classType)
        {
            if (onAfterInsert != null) onAfterInsert(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterInsert(sender, classType.Name);

            if (onAfterComplete != null) onAfterComplete(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterComplete(sender, classType.Name);

            if (Context.CacheManager != null)
                Context.CacheManager.Insert(sender, classType.Name);
        }

        public void BeforeInsertDocument(object sender, MongoMapper.OnBeforeInsertEventHandler onBeforeInsert,
                                         Type classType)
        {
            if (onBeforeInsert != null) onBeforeInsert(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnBeforeInsert(sender, classType.Name);
        }

        public void AfterUpdateDocument(object sender, MongoMapper.OnAfterModifyEventHandler onAfterModify,
                                        MongoMapper.OnAfterCompleteEventHandler onAfterComplete, Type classType)
        {
            if (onAfterModify != null) onAfterModify(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterModify(sender, classType.Name);

            if (onAfterComplete != null) onAfterComplete(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterComplete(sender, classType.Name);


            if (Context.CacheManager != null)
                Context.CacheManager.Update(sender, classType.Name);
        }

        public void BeforeUpdateDocument(object sender, MongoMapper.OnBeforeModifyEventHandler onBeforeModify,
                                         Type classType)
        {
            if (onBeforeModify != null) onBeforeModify(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnBeforeModify(sender, classType.Name);
        }

        public void AfterDeleteDocument(object sender, MongoMapper.OnAfterDeleteEventHandler onAfterDelete,
                                        MongoMapper.OnAfterCompleteEventHandler onAfterComplete, Type classType)
        {
            if (onAfterDelete != null) onAfterDelete(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterDelete(sender, classType.Name);


            if (onAfterComplete != null) onAfterComplete(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnAfterComplete(sender, classType.Name);

            if (Context.CacheManager != null)
                Context.CacheManager.Delete(sender, classType.Name);
        }

        public void BeforeDeleteDocument(object sender, MongoMapper.OnBeforeDeleteEventHandler onBeforeDelete,
                                         Type classType)
        {
            if (onBeforeDelete != null) onBeforeDelete(sender, new EventArgs());

            if (Context.Rules != null)
                Context.Rules.OnBeforeDelete(sender, classType.Name);
        }

        #endregion
    }
}