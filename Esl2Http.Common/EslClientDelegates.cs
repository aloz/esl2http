namespace Esl2Http.Common
{
    public class EslClientDelegates
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
