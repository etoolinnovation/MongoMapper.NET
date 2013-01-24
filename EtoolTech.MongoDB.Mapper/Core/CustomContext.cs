using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper
{
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
            get { return _cacheManager; }
        }

        public static IConfig Config
        {
            get { return _config; }
        }

        public static IRules Rules
        {
            get { return _rulesManager; }
        }

        #endregion

        #region Public Methods

        public static void SetCacheManagerInterface(ICache CustomCacheManager)
        {
            _cacheManager = CustomCacheManager;
        }

        public static void SetDatabaseConfigInterface(IConfig CustomConfig)
        {
            _config = CustomConfig;
        }

        public static void SetRulesManagerInterface(IRules CustomRules)
        {
            _rulesManager = CustomRules;
        }

        #endregion
    }
}