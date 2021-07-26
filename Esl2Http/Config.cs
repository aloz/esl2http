using Esl2Http.Dal;
using Esl2Http.Interfaces;
using System;

namespace Esl2Http
{
    static class Config
    {
        const string CONST_ENV_PREFIX = "esl2http_";

        static Tuple<int?> _ConfigFromDb;

        static Config()
        {
            // hardcoded for postgres dal
            using (IDal dal = new DalPostgres(DbConnectionString))
            {
                _ConfigFromDb = dal.GetConfig();
            }
        }
        
        public static int EslRxBufferSize
        {
            get
            {
                int result = 4096;
                string envval = Environment.GetEnvironmentVariable($"{CONST_ENV_PREFIX}EslRxBufferSize");
                if (!string.IsNullOrEmpty(envval))
                {
                    int.TryParse(envval, out result);
                }
                return result;
            }
        }

        public static string EslEventsToSubscribe
        {
            get
            {
                string result = Environment.GetEnvironmentVariable($"{CONST_ENV_PREFIX}EslEventsToSubscribe");
                if (string.IsNullOrEmpty(result))
                    result = "CHANNEL_ORIGINATE CHANNEL_ANSWER CHANNEL_HANGUP";
                return result;
            }
        }

        public static string DbConnectionString
        {
            get
            {
                string result = Environment.GetEnvironmentVariable($"{CONST_ENV_PREFIX}DBConnectionString");
                if (string.IsNullOrEmpty(result))
                    result = "Host=localhost;Username=esl2http;Password=esl2http;Database=esl2http";
                return result;
            }
        }

        public static int? HttpTimeoutS
        {
            get
            {
                return _ConfigFromDb.Item1;
            }
        }
    }
}