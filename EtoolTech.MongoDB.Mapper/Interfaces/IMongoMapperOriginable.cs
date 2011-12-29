namespace EtoolTech.MongoDB.Mapper.Interfaces
{    
    public interface IMongoMapperStringOriginalObject
    {
        string StringOriginalObject { get; set; }
    }

    public interface IMongoMapperOriginable : IMongoMapperStringOriginalObject
    {
        T GetOriginalObject<T>();

        T GetOriginalT<T>();        
    }
}