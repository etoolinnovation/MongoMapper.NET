namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperOriginable
    {
        #region Public Methods

        T GetOriginalObject<T>();

        T GetOriginalT<T>();

        void SaveOriginal(bool Force);

        #endregion
    }
}