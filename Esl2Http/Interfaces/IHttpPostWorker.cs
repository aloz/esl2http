using System;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Interfaces
{
    interface IHttpPostWorker : IDisposable
    {
        void Start(
            LogDelegate LogDelegate,
            int? HttpTimeoutS,
            string ConnectionString);

        void Stop();
    }
}