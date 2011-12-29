namespace EtoolTech.MongoDB.Mapper.Configuration
{
    public class ConfigManager
    {
        internal static readonly MongoMapperConfiguration Config = MongoMapperConfiguration.GetConfig();

        internal static readonly string DataBaseName = CustomContext.Config == null
                                                           ? Config.Database.Name
                                                           : CustomContext.Config.Database;

        internal static readonly string Host = CustomContext.Config == null
                                                   ? Config.Server.Host
                                                   : CustomContext.Config.Database;

        internal static readonly int Port = CustomContext.Config == null
                                                ? Config.Server.Port
                                                : CustomContext.Config.Port;

        internal static readonly int PoolSize = CustomContext.Config == null
                                                    ? Config.Server.PoolSize
                                                    : CustomContext.Config.PoolSize;

        internal static readonly string UserName = CustomContext.Config == null
                                                       ? Config.Database.User
                                                       : CustomContext.Config.UserName;

        internal static readonly string PassWord = CustomContext.Config == null
                                                       ? Config.Database.Password
                                                       : CustomContext.Config.PassWord;

        internal static readonly int WaitQueueTimeout = CustomContext.Config == null
                                                            ? Config.Server.WaitQueueTimeout
                                                            : CustomContext.Config.WaitQueueTimeout;


        internal static readonly int MaxDocumentSize = CustomContext.Config == null
                                                           ? Config.Context.MaxDocumentSize
                                                           : CustomContext.Config.MaxDocumentSize;

        internal static readonly bool SafeMode = CustomContext.Config == null
                                                     ? Config.Context.SafeMode
                                                     : CustomContext.Config.SafeMode;

        internal static readonly bool FSync = CustomContext.Config == null
                                                  ? Config.Context.FSync
                                                  : CustomContext.Config.FSync;

        internal static readonly bool ExceptionOnDuplicateKey = CustomContext.Config == null
                                                                    ? Config.Context.ExceptionOnDuplicateKey
                                                                    : CustomContext.Config.ExceptionOnDuplicateKey;

        internal static readonly bool EnableOriginalObject = CustomContext.Config == null
                                                                 ? Config.Context.EnableOriginalObject
                                                                 : CustomContext.Config.EnableOriginalObject;

        internal static readonly bool UserIncrementalId = CustomContext.Config == null
                                                              ? Config.Context.UserIncrementalId
                                                              : CustomContext.Config.UserIncrementalId;
    }
}