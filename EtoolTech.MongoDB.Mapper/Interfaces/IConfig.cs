namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IConfig
    {
        string Database { get; }
        string ConnectionString { get; }
    }
}