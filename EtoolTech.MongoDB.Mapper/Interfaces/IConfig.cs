namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IConfig
    {
        #region Public Properties
      
        string Database { get; }

        bool EnableOriginalObject { get; }

        bool ExceptionOnDuplicateKey { get; }

        string Url { get; }

        int MaxDocumentSize { get; }

        bool UseChildIncrementalId { get; }

        bool UseIncrementalId { get; }


        #endregion
    }
}