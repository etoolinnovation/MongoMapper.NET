namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IMongoMapperOriginable 
    {
        T GetOriginalObject<T>();

        T GetOriginalT<T>();

        void SaveOriginal();
    }
}