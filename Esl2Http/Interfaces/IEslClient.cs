using System;
using static Esl2Http.Delegates.EslClientDelegates;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Interfaces
{
    interface IEslClient : IDisposable
    {
        void Start(
            LogDelegate LogDelegate,
            EslClientResponseEslEventDelegate ResponseEslEventDelegate,
            string EslEventsList,
            int RxBufferSize);

        void Stop();
    }
}