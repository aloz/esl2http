using System;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http.Interfaces
{
    interface IEslClient : IDisposable
    {
        void Start(
            EslClientLogDelegate LogDelegate,
            EslClientResponseEslEventDelegate ResponseEslEventDelegate,
            string EslEventsList,
            int RxBufferSize);

        void Stop();
    }
}