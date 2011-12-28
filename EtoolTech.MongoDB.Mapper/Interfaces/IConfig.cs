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

        int WaitQueueTimeout { get; }

        bool SafeMode { get; }

        bool FSync { get; }

        bool ExceptionOnDuplicateKey { get; }

        bool EnableOriginalObject { get; }

        bool UserIncrementalId { get; }
    }
}