using System;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http.Interfaces
{
    interface IHttpPostWorker : IDisposable
    {
        void Start(
            EslClientLogDelegate LogDelegate,
            int? HttpTimeoutS,
            string ConnectionString);

        void Stop();
    }
}
