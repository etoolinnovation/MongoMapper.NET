namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IConfig
    {
        string Database { get; }
        string Host { get; }
        int Port { get; }
        string UserName { get; }
        string PassWord { get; }
        int PoolSize { get; }
    }
}