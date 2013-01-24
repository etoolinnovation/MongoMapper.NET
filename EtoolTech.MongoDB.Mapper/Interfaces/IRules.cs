namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IRules
    {
        #region Public Methods

        void OnAfterComplete(object Sender, string ClassName);

        void OnAfterDelete(object Sender, string ClassName);

        void OnAfterInsert(object Sender, string ClassName);

        void OnAfterModify(object Sender, string ClassName);

        void OnBeforeDelete(object Sender, string ClassName);

        void OnBeforeInsert(object Sender, string ClassName);

        void OnBeforeModify(object Sender, string ClassName);

        #endregion
    }
}