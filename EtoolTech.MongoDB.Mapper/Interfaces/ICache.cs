namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface ICache
    {
        #region Public Methods

        void Delete(object Obj, string ClassName);

        void Insert(object Obj, string ClassName);

        void Update(object Obj, string ClassName);

        #endregion
    }
}