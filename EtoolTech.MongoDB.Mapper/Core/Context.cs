using EtoolTech.MongoDB.Mapper.Interfaces;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class Context
    {
        private static IConfig _config;
        private static ICache _cacheManager;
        private static IRules _rulesManager;

        public static IConfig Config
        {
            get { return _config; }
        }

        public static ICache CacheManager
        {
            get { return _cacheManager; }
        }

        public static IRules Rules
        {
            get { return _rulesManager; }
        }

        public static void SetDatabaseConfigInterface(IConfig config)
        {
            _config = config;
        }

        public static void SetCacheManagerInterface(ICache cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public static void SetRulesManagerInterface(IRules rules)
        {
            _rulesManager = rules;
        }
        
    }
}