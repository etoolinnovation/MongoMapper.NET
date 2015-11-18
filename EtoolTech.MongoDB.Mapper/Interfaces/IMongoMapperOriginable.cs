namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperOriginable<T>
    {
        #region Public Methods

        T GetOriginalObject();

        T GetOriginalT();

        void SaveOriginal(bool Force);

        #endregion
    }
}