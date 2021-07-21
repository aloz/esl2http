namespace Esl2Http.Delegates
{
    class EslClientDelegates
    {
        public enum EslClientLogType
        {
            Information,
            Warning,
            Error,
            Critical
        }

        public delegate void EslClientLogDelegate(string str, EslClientLogType LogType = EslClientLogType.Information);
        public delegate void EslClientResponseEslEventDelegate(string str);
    }
}