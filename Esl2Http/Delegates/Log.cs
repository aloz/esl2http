using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esl2Http.Delegates
{
    class Log
    {
        public enum LogType
        {
            Information,
            Warning,
            Error,
            Critical
        }

        public delegate void LogDelegate(Type sender, string str, LogType LogType = LogType.Information);
    }
}