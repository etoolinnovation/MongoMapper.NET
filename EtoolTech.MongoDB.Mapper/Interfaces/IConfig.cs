namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IConfig
    {
        string Database { get; }

        string Host { get; }

        string Port { get; }

        string ReplicaSetName { get; }

        int MinReplicaServersToWrite { get; }

        bool BalancedReading { get; }

        string UserName { get; }

        string PassWord { get; }

        int PoolSize { get; }

        int WaitQueueTimeout { get; }

        bool SafeMode { get; }

        bool FSync { get; }

        bool ExceptionOnDuplicateKey { get; }

        bool EnableOriginalObject { get; }

        bool UseIncrementalId { get; }
		
		bool UseChildIncrementalId { get; }

        int MaxDocumentSize { get; }
    }
}