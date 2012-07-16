namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IConfig
    {
        #region Public Properties

        bool BalancedReading { get; }

        string Database { get; }

        bool EnableOriginalObject { get; }

        bool ExceptionOnDuplicateKey { get; }

        bool FSync { get; }

        string Host { get; }

        int MaxDocumentSize { get; }

        int MinReplicaServersToWrite { get; }

        string PassWord { get; }

        int PoolSize { get; }

        string ReplicaSetName { get; }

        bool SafeMode { get; }

        bool UseChildIncrementalId { get; }

        bool UseIncrementalId { get; }

        string UserName { get; }

        int WaitQueueTimeout { get; }

        #endregion
    }
}