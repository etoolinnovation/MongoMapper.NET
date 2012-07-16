namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface ICache
    {
        #region Public Methods

        void Delete(object obj, string className);

        void Insert(object obj, string className);

        void Update(object obj, string className);

        #endregion
    }
}