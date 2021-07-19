using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esl2Http
{
    static class Config
    {
        const string CONST_ENV_PREFIX = "esl2http_";
        public static int EslRxBufferSize
        {
            get
            {
                int result = 4096;
                string envval = Environment.GetEnvironmentVariable($"{CONST_ENV_PREFIX}EslRxBufferSize");
                if(!string.IsNullOrEmpty(envval))
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
    }
}