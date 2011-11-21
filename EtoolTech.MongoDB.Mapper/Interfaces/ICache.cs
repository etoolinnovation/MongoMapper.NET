namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface ICache
    {
        void Insert(object obj, string className);
        void Update(object obj, string className);
        void Delete(object obj, string className);
    }
}