namespace EtoolTech.MongoDB.Mapper
{
    using EtoolTech.MongoDB.Mapper.Interfaces;

    public class CustomContext
    {
        #region Constants and Fields

        private static ICache _cacheManager;

        private static IConfig _config;

        private static IRules _rulesManager;

        #endregion

        #region Public Properties

        public static ICache CacheManager
        {
            get
            {
                return _cacheManager;
            }
        }

        public static IConfig Config
        {
            get
            {
                return _config;
            }
        }

        public static IRules Rules
        {
            get
            {
                return _rulesManager;
            }
        }

        #endregion

        #region Public Methods

        public static void SetCacheManagerInterface(ICache cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public static void SetDatabaseConfigInterface(IConfig config)
        {
            _config = config;
        }

        public static void SetRulesManagerInterface(IRules rules)
        {
            _rulesManager = rules;
        }

        #endregion
    }
}